using System;
using System.Messaging;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqUtil
    {
        public static string GetQueuePath(Endpoint endpoint)
        {
            var uri = endpoint.Uri;
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
            if (queue == null)
                return null;
            return new Uri("msmq://" + queue.MachineName + "/" +
                queue.QueueName.Split('\\')[1]);
        }

		public static MessageQueue CreateQueue(string queuePath,QueueAccessMode accessMode)
		{
			try
			{
				if (MessageQueue.Exists(queuePath) == false)
				{
					try
					{
						MessageQueue.Create(queuePath, true);
					}
					catch (Exception e)
					{
						throw new TransportException("Queue " + queuePath + " doesn't exists and we failed to create it", e);
					}
				}

				return new MessageQueue(queuePath, accessMode);
			}
			catch (Exception e)
			{
				throw new MessagePublicationException("Could not open queue (" + queuePath + ")", e);
			}
		}
    }
}