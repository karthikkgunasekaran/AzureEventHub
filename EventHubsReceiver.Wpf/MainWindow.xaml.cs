using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace EventHubsReceiver.Wpf
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
        object _itemsLock = new object ();

        public MainWindow()
        {
            viewModel = new EventDataViewModel();
            this.DataContext = viewModel;
            InitializeComponent();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() => StopProcessing());
            eventDataGrid.ItemsSource = viewModel.EventDataList;
            BindingOperations.EnableCollectionSynchronization(viewModel.EventDataList, _itemsLock);
            lock (_itemsLock)
            {
                viewModel.EventDataList.Add(new EventData { OperationName = "text" });
            }
            StartProcessing().Wait();
        }

        private async Task StartProcessing()
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageClient = new BlobContainerClient(blobConnectionString, blobContainerName);
            eventProcessorClient = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);

            eventProcessorClient.ProcessEventAsync += processEventHandler;
            eventProcessorClient.ProcessErrorAsync += processErrorHandler;

            await eventProcessorClient.StartProcessingAsync();
        }

        private void StopProcessing()
        {
            Console.WriteLine("Stop Processing Requested at {0}.", DateTime.Now);
            eventProcessorClient.StopProcessingAsync().Wait();

            eventProcessorClient.ProcessEventAsync -= processEventHandler;
            eventProcessorClient.ProcessErrorAsync -= processErrorHandler;

            Console.WriteLine("Stop Processing Completed at {0}.", DateTime.Now);
        }

        private async Task processEventHandler(ProcessEventArgs arg)
        {
            try
            {
                var operationName = string.Format("\t Received Event:{0}", Encoding.UTF8.GetString(arg.Data.EventBody.ToArray()));
                lock (_itemsLock)
                {
                    viewModel.EventDataList.Add(new EventData { OperationName = operationName });
                }
                await arg.UpdateCheckpointAsync(arg.CancellationToken);
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("Error Occurred: {0}", ex.Message));
            }
          
        }

        private Task processErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine("\t Error Occured. Exception Details {0}, {1}", arg.Exception.Message, arg.Exception.StackTrace);
            return Task.CompletedTask;
        }
    }
}
