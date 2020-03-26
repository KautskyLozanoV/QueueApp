using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueApp
{
    class Program
    {
        private const string ConnectionString = "";
        static async Task Main(string[] args)
        {
            string message = null;
            if (args.Any())
            {
                message = String.Join(" ", args);
                await SendArticleAsync(message);
                Console.WriteLine($"Sent: {message}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
            }

        }

        static async Task SendArticleAsync(string newsMessage)
        {
            var queue = GetQueue();
            bool created = await queue.CreateIfNotExistsAsync();
            if (created)
            {
                Console.WriteLine("Queue created");
            }

            var queueMessage = new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(queueMessage);
        }

        static async Task<string> ReceiveArticleAsync()
        {
            var queue = GetQueue();
            bool exists = await queue.ExistsAsync();
            if (exists)
            {
                var cloudMessage = await queue.GetMessageAsync();
                string message = cloudMessage.AsString;
                await queue.DeleteMessageAsync(cloudMessage);
                return message;
            }

            return "<queue empty or not created>";
        }

        static CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference("newsqueue");
        }
    }
}
