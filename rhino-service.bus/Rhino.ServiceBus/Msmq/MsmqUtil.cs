using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqUtil
    {
        public static string GetQueuePath(string uri)
        {
            return GetQueuePath(new Uri(uri));
        }

        public static string GetQueuePath(Uri uri)
        {
            if (uri.AbsolutePath.IndexOf("/", 1) >= 0)
            {
                throw new InvalidOperationException(
                    "Invalid enpoint url : " + uri + Environment.NewLine +
                    "Queue Endpoints can't have a child folders (including 'public')" + Environment.NewLine +
                    "Good: 'msmq://machinename/queue_name'." + Environment.NewLine +
                    " Bad: msmq://machinename/round_file/queue_name"
                    );
            }


            string localhost = Environment.MachineName.ToLowerInvariant();

            string hostName = uri.Host;
            if (string.Compare(hostName, ".") == 0 || 
                string.Compare(hostName, "localhost", true) == 0)
            {
                hostName = localhost;
                uri = new Uri("msmq://" + localhost + uri.AbsolutePath);
            }

            return string.Format(hostName + "\\private$\\" + uri.AbsolutePath.Substring(1));
        }

        public static Uri GetQueueUri(MessageQueue queue)
        {
            return new Uri("msmq://"+queue.MachineName+"/"+
                queue.QueueName.Split('\\')[1]);
        }
    }
}