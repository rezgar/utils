using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rezgar.Utils.MessageQueue
{
    public interface IMessageQueue
    {
        Task SendMessageAsync(Message message, TimeSpan? timeToLive = null);
        Task<IList<Message>> GetMessagesAsync(int? batchSize = null, string target = null);
    }
}
