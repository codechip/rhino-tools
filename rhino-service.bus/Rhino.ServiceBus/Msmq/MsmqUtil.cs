using System;
using System.Messaging;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
	using System.Text.RegularExpressions;

	public class MsmqUtil
	{
		public static QueueInfo GetQueuePath(Endpoint endpoint)
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

			string hostName = uri.Host;
			if (string.Compare(hostName, ".") == 0 ||
				string.Compare(hostName, Environment.MachineName, true) == 0 ||
				string.Compare(hostName, "localhost", true) == 0)
			{
				return  new QueueInfo
				{
					IsLocal = true,
					QueuePath = @".\private$\" + uri.AbsolutePath.Substring(1),
                    QueueUri = uri
				};
			}

			return new QueueInfo
			{
				IsLocal = false,
				QueuePath = "FormatName:DIRECT=TCP:" + hostName + @"\private$\" + uri.AbsolutePath.Substring(1),
                QueueUri = uri
			};
		}

		static readonly Regex queuePath = new Regex(@"FormatName:DIRECT=(?<transport>\w+):(?<machineName>[\w\d-_.#$;]+)\\private\$\\(?<queueName>[\w\d-_.#$;]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static Uri GetQueueUri(MessageQueue queue)
		{
			if (queue == null)
				return null;
			var match = queuePath .Match(queue.Path);
			return new Uri("msmq://" + match.Groups["machineName"] + "/" +
				match.Groups["queueName"]);
		}

		public static MessageQueue CreateQueue(string queuePath, QueueAccessMode accessMode)
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