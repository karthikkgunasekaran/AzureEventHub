using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubsSender
{
    class Program
    {
        private const string connectionString = "<tobefilled>";
        private const string eventHubName = "<tobefilled>";

        static async Task Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            EventSender(cancellationTokenSource.Token);

            Console.WriteLine("Enter any key to stop sending events!");
            Console.ReadLine();

            cancellationTokenSource.Cancel();
            Console.WriteLine("Cancellation Requested at {0}.", DateTime.Now);

            //Giving time for EventSender method to process the cancellation 
            await Task.Delay(TimeSpan.FromSeconds(65));
            Console.WriteLine("Exiting main program at {0}.", DateTime.Now);
        }

        private static async Task EventSender(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event Sender Started at {0}.", DateTime.Now);
            do
            {
                if (cancellationToken.IsCancellationRequested)
                    continue;

                await PublishEvents(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(60));
            } while (!cancellationToken.IsCancellationRequested);

            Console.WriteLine("Event Sender Stopped at {0}.", DateTime.Now);
        }

        private static async Task PublishEvents(CancellationToken cancellationToken)
        {
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                var dateTime = DateTime.Now;

                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(string.Format("First event at {0}", dateTime))));
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(string.Format("Second event at {0}", dateTime))));
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(string.Format("Third event at {0}", dateTime))));

                if (cancellationToken.IsCancellationRequested)
                    return;

                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 3 events has been published at {0}.", dateTime);
            }
        }
    }
}
