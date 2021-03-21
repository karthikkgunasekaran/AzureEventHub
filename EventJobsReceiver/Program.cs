using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace EventJobsReceiver
{
    class Program
    {
        private const string eventHubConnectionString = "<tobefilled>";
        private const string eventHubName = "<tobefilled>";
        private const string blobConnectionString = "<tobefilled>";
        private const string blobContainerName = "<tobefilled>";

       static async Task Main(string[] args)
        {

            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageClient = new BlobContainerClient(blobConnectionString, blobContainerName);
            var eventProcessorClient = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);

            eventProcessorClient.ProcessEventAsync += processEventHandler;
            eventProcessorClient.ProcessErrorAsync += processErrorHandler;

            await eventProcessorClient.StartProcessingAsync();

            await Task.Delay(TimeSpan.FromMinutes(1));

            await eventProcessorClient.StopProcessingAsync();
        }

        private static Task processErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine("\t Error Occured. Exception Details {0}, {1}", arg.Exception.Message, arg.Exception.StackTrace);
            return Task.CompletedTask;
        }

        private static async Task processEventHandler(ProcessEventArgs arg)
        {
            Console.WriteLine("\t Received Event:{0}", Encoding.UTF8.GetString(arg.Data.EventBody.ToArray()));
            await arg.UpdateCheckpointAsync(arg.CancellationToken);
        }
    }
}
