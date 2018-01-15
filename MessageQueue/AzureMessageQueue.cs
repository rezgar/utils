using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Rezgar.Utils.MessageQueue
{
    public class AzureMessageQueue : IMessageQueue
    {
        public const int DefaultBatchSize = 30;

        private readonly CloudQueueClient _cloudQueueClient;
        private readonly CloudQueue _cloudQueue;

        public AzureMessageQueue(string queueName, CloudStorageAccount cloudStorageAccount)
        {
            _cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _cloudQueue = _cloudQueueClient.GetQueueReference(queueName);
        }

        public Task SendMessageAsync(Message message, TimeSpan? timeToLive)
        {
            return _cloudQueue.AddMessageAsync(
                    new CloudQueueMessage(Message.Serialize(message)),
                    timeToLive,
                    null,
                    new QueueRequestOptions { },
                    new Microsoft.WindowsAzure.Storage.OperationContext { }
                );
        }

        public Task<IList<Message>> GetMessagesAsync(int? batchSize = null, string target = null)
        {
            return _cloudQueue.GetMessagesAsync(batchSize ?? DefaultBatchSize)
                    .ContinueWith(task =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion)
                            return task.Result.Select(azureMessage => Message.Deserialize(azureMessage.AsString)).ToArray() as IList<Message>;
                        else
                        {
                            Trace.TraceError("AzureMessageQueue.GetMessages: Failed for queue '{0}' with exception [{1}]", _cloudQueue.Name, task.Exception);
                            throw task.Exception;
                        }
                    });
        }
    }
}
