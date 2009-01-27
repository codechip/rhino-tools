using System;
using System.Messaging;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
	using System.Collections.Generic;

	/// <summary>
	/// Default subqueue stragey enabled in MSMQ 4.0
	/// </summary>
	public class SubQueueStrategy : IQueueStrategy
	{
		public MessageQueue InitializeQueue(Uri endpoint)
		{
			var queue = endpoint.CreateQueue(QueueAccessMode.SendAndReceive);
			return queue;
		}

		/// <summary>
		/// Creates the subscription queue URI.
		/// </summary>
		/// <param name="subscriptionQueue">The subscription queue.</param>
		/// <returns></returns>
		public Uri CreateSubscriptionQueueUri(Uri subscriptionQueue)
		{
			return new Uri(subscriptionQueue + ";subscriptions");
		}

		/// <summary>
		/// Gets a listing of all timeout messages.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeoutInfo> GetTimeoutMessages(MessageQueue queue)
		{
			var fullSubQueueName = queue.Path + ";timeout";
			using (var timeoutQueue = new MessageQueue(fullSubQueueName, QueueAccessMode.Receive))
			{
				timeoutQueue.MessageReadPropertyFilter.Extension = true;

				var enumerator2 = timeoutQueue.GetMessageEnumerator2();
				while(enumerator2.MoveNext())
				{
					var message = enumerator2.Current;
					if(message==null)
						continue;
					
					yield return new TimeoutInfo
					{
						Id = message.Id,
						Time = DateTime.FromBinary(BitConverter.ToInt64(message.Extension, 0))
					};
				}
			}
		}

		/// <summary>
		/// Moves the message from the timeout queue to the main queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="messageId">The message id.</param>
		public void MoveTimeoutToMainQueue(MessageQueue queue, string messageId)
		{
			var fullSubQueueName = queue.Path + ";timeout";
			using (var timeoutQueue = new MessageQueue(fullSubQueueName, QueueAccessMode.Receive))
			{
				var message = timeoutQueue.ReceiveById(messageId, queue.GetTransactionType());
				message.AppSpecific = 0;//reset timeout flag
				queue.Send(message, queue.GetTransactionType());
			}
		}

	    public bool TryMoveMessage(MessageQueue queue, Message message, SubQueue subQueue, out string msgId)
	    {
	        try
	        {
	            queue.MoveToSubQueue(subQueue.ToString(), message);
	            msgId = message.Id;
	            return true;
	        }
	        catch (TransportException)
	        {
	            msgId = null;
	            return false;
	        }
	    }
	}
}
