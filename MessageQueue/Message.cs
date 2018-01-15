using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.MessageQueue
{
    public class Message
    {
        public string Text;

        /// <summary>
        /// Sender machine name
        /// </summary>
        public string Sender;
        /// <summary>
        /// Target machine name
        /// </summary>
        public string Target;

        public Message(string text, string sender, string target = null)
        {
            Text = text;
            Sender = sender;
            Target = target;
        }
        
        public static string Serialize(Message message)
        {
            return string.Join(Environment.NewLine,
                message.Text,
                message.Sender,
                message.Target
            );
        }

        public static Message Deserialize(string message)
        {
            var parts = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            return new Message
            (
                parts[0],
                parts[1],
                parts[2]
            );
        }
    }
}
