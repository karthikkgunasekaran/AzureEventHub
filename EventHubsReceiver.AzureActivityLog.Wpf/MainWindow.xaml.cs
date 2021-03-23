using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventHubsReceiver.AzureActivityLog.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string eventHubConnectionString = "<tobefilled>";
        private const string eventHubName = "<tobefilled>";
        private const string blobConnectionString = "<tobefilled>";
        private const string blobContainerName = "<tobefilled>";
        private EventProcessorClient eventProcessorClient;
        private EventDataViewModel viewModel;
        object _itemsLock = new object();

        public MainWindow()
        {
            viewModel = new EventDataViewModel();
            InitializeComponent();
            this.DataContext = viewModel;
            this.Closing += OnClosing;
            eventDataGrid.ItemsSource = viewModel.EventDataList;

            BindingOperations.EnableCollectionSynchronization(viewModel.EventDataList, _itemsLock);
            StartEventProcessing().Wait();
        }

        private async Task StartEventProcessing()
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageClient = new BlobContainerClient(blobConnectionString, blobContainerName);
            eventProcessorClient = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);

            eventProcessorClient.ProcessEventAsync += processEventHandler;
            eventProcessorClient.ProcessErrorAsync += processErrorHandler;

            await eventProcessorClient.StartProcessingAsync();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show(String.Format("EventProcessorClient StopProcessing() Called at {0}.", DateTime.Now));

            //TODO: This is taking long time to complete (around 1 min), need to analyze why. This inturn freezes the window, till it is completed.
            eventProcessorClient.StopProcessingAsync().Wait();

            eventProcessorClient.ProcessEventAsync -= processEventHandler;
            eventProcessorClient.ProcessErrorAsync -= processErrorHandler;

            MessageBox.Show(String.Format("EventProcessorClient StopProcessing() Completed at {0}", DateTime.Now));
            e.Cancel = false;
        }

        #region Event Processor Client CallBacks

        /// <summary>
        /// Processes the events from Azure Event Hubs
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task processEventHandler(ProcessEventArgs arg)
        {
            try
            {
                if (arg.Data == null || arg.Data.EventBody == null)
                {
                    await arg.UpdateCheckpointAsync(arg.CancellationToken);
                    return;
                }

                var eventData = GetEventData(arg);
                if(eventData != null)
                {
                    lock (_itemsLock)
                    {
                        viewModel.EventDataList.Add(eventData);
                        viewModel.EventDataList.OrderByDescendingCustom(x => x.TimeStamp);
                    }
                }
              
                await arg.UpdateCheckpointAsync(arg.CancellationToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error Occurred: {0}", ex.Message));
            }

        }

        /// <summary>
        /// Processes the error
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task processErrorHandler(ProcessErrorEventArgs arg)
        {
            MessageBox.Show(String.Format("Error Occured. Exception Details {0}, {1}", arg.Exception.Message, arg.Exception.StackTrace));
            return Task.CompletedTask;
        }

        #endregion Event Processor Client CallBacks


        #region Helpers

        /// <summary>
        /// Get Event Data from <see cref="ProcessEventArgs"/>
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private EventData GetEventData(ProcessEventArgs arg)
        {
            var body = Encoding.UTF8.GetString(arg.Data.EventBody.ToArray());
            var record = GetActivityRecord(body);

            return new EventData
            {
                OperationName = record?.operationName,
                InitiatedBy = record?.identity?.claims?.HttpSchemasXmlsoapOrgWs200505IdentityClaimsEmailaddress,
                Status = record?.resultType,
                Subscription = record?.properties.hierarchy,
                TimeStamp = arg.Data.EnqueuedTime.DateTime,
                TimeStampFormatted = GetFormattedTimeStamp(arg.Data.EnqueuedTime.DateTime.ToLocalTime())
            };
        }

        private string GetFormattedTimeStamp(DateTime time)
        {
            return time.ToString("dddd MMM dd yyyy HH:mm:ss K");
        }

        /// <summary>
        /// Deserializes the event body for <see cref="ActivityLog"/> and returns the last record (response)
        /// Returns null if the event body can not deserialized to <see cref="ActivityLog"/>
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private Record GetActivityRecord(string body)
        {
            try
            {
                var activtyLog = JsonConvert.DeserializeObject<ActivityLog>(body, new JsonSerializerSettings
                                {
                                    Error = HandleDeserializationError
                                });
                if (activtyLog != null && activtyLog.records?.Count > 0)
                    return activtyLog.records[activtyLog.records.Count - 1];
            }
            catch(Exception)
            {
                // Don't do anything
            }

            return null;
        }

        private void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            errorArgs.ErrorContext.Handled = true;
        }
        #endregion Helpers
    }
}
