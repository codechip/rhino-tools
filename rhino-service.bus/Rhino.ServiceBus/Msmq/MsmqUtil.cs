using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqUtil
    {
        public static QueueDescriptor GetQueueDescription(string uri)
        {
            return GetQueueDescription(new Uri(uri));
        }

        public static QueueDescriptor GetQueueDescription(Uri uri)
        {
            var descriptor = new QueueDescriptor();

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
                descriptor.IsLocal = true;
            }
            else
            {
                descriptor.IsLocal = string.Compare(uri.Host, localhost, true) == 0;
            }

            descriptor.Uri = uri;
            descriptor.FullQueuePath = string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));
            descriptor.QueuePath = string.Format(hostName + "\\private$\\" + uri.AbsolutePath.Substring(1));
            return descriptor;
        }

        public static Uri GetQueueUri(MessageQueue queue)
        {
            return new Uri("msmq://"+queue.MachineName+"/"+
                queue.QueueName.Split('\\')[1]);
        }
    }
}