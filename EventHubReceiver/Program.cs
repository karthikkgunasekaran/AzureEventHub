using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace EventHubReceiver
{
    class Program
    {
        private const string eventHubConnectionString = "<tobefilled>";
        private const string eventHubName = "<tobefilled>";
        private const string blobConnectionString = "<tobefilled>";
        private const string blobContainerName = "<tobefilled>";
        private static EventProcessorClient eventProcessorClient;

       static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() => StopProcessing());

            await StartProcessing();

            Console.WriteLine("Enter any key to stop sending events!");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
            Console.WriteLine("Cancellation Requested at {0}.", DateTime.Now);

            //Giving time for eventProcessorClient to finish 
            await Task.Delay(TimeSpan.FromSeconds(30));

            Console.WriteLine("Exiting main program at {0}.", DateTime.Now);
        }

        private static async Task StartProcessing()
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageClient = new BlobContainerClient(blobConnectionString, blobContainerName);
            eventProcessorClient = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);

            eventProcessorClient.ProcessEventAsync += processEventHandler;
            eventProcessorClient.ProcessErrorAsync += processErrorHandler;

            await eventProcessorClient.StartProcessingAsync();
        }

        private static void StopProcessing()
        {
            Console.WriteLine("Stop Processing Requested at {0}.", DateTime.Now);
            eventProcessorClient.StopProcessingAsync().Wait();

            eventProcessorClient.ProcessEventAsync -= processEventHandler;
            eventProcessorClient.ProcessErrorAsync -= processErrorHandler;

            Console.WriteLine("Stop Processing Completed at {0}.", DateTime.Now);
        }
   
        private static async Task processEventHandler(ProcessEventArgs arg)
        {
            Console.WriteLine("\t Received Event:{0}", Encoding.UTF8.GetString(arg.Data.EventBody.ToArray()));
            await arg.UpdateCheckpointAsync(arg.CancellationToken);
        }

        private static Task processErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine("\t Error Occured. Exception Details {0}, {1}", arg.Exception.Message, arg.Exception.StackTrace);
            return Task.CompletedTask;
        }

    }
}
