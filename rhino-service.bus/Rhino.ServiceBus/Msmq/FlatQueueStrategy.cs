using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
	using System.Collections.Generic;

	/// <summary>
	/// Handles message moving to sibling queues.
	/// Suitable for MSMQ 3.0
	/// </summary>
	/// <remarks>
	/// <para>
	/// This strategy presumes additional queues than those defined by the endpoint.
	/// </para>
	/// <list type="bullet">
	/// <listheader>So your queue structure would be:</listheader>
	/// <item>[my_queue_name]</item>
	/// <item>[my_queue_name]<c>#subscriptions</c></item>
	/// <item>[my_queue_name]<c>#errors</c></item>
	/// <item>[my_queue_name]<c>#discarded</c></item>
	/// <item>[my_queue_name]<c>#timeout</c></item>
	/// </list>
	/// </remarks>
	public class FlatQueueStrategy : IQueueStrategy
	{
		private readonly Uri endpoint;
		private const string subscriptions = "#subscriptions";
		private const string errors = "#errors";
		private const string timeout = "#timeout";
		private const string discarded = "#discarded";

		/// <summary>
		/// Initializes a new instance of the <see cref="FlatQueueStrategy"/> class.
		/// </summary>
		/// <param name="endpoint">The endpoint.</param>
		public FlatQueueStrategy(Uri endpoint)
		{
			this.endpoint = endpoint;
		}

		public MessageQueue InitializeQueue(Uri endpoint)
		{
			var accessMode = QueueAccessMode.SendAndReceive;
			
			var root = endpoint.CreateQueue(accessMode);

			MsmqUtil.CreateQueue(GetSubscriptionQueuePath(), accessMode);
			MsmqUtil.CreateQueue(GetErrorsQueuePath(), accessMode);
			MsmqUtil.CreateQueue(GetDiscardedQueuePath(), accessMode);
			MsmqUtil.CreateQueue(GetTimeoutQueuePath(), accessMode);
			return root;
		}

		/// <summary>
		/// Creates the subscription queue URI.
		/// </summary>
		/// <param name="subscriptionQueue">The subscription queue.</param>
		/// <returns></returns>
		public Uri CreateSubscriptionQueueUri(Uri subscriptionQueue)
		{
			return new Uri(subscriptionQueue + "#subscriptions");
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to subscription queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToSubscriptionQueue(MessageQueue queue, Message message)
		{
			//the endpoint IS the subscription queue
			using (var destinationQueue = new MessageQueue(GetSubscriptionQueuePath(), QueueAccessMode.Send))
			{
				destinationQueue.Send(queue.ReceiveByLookupId(message.LookupId),
														  queue.GetTransactionType());
			}
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to errors queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public string MoveToErrorsQueue(MessageQueue queue, Message message)
		{
			using (var destinationQueue = new MessageQueue(GetErrorsQueuePath(), QueueAccessMode.Send))
			{
			    var sentMessage = queue.ReceiveByLookupId(message.LookupId);
			    destinationQueue.Send(sentMessage, queue.GetTransactionType());
			    return sentMessage.Id;
			}
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to the timeout queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToTimeoutQueue(MessageQueue queue, Message message)
		{
			using (var destinationQueue = new MessageQueue(GetTimeoutQueuePath(), QueueAccessMode.Send))
			{
				destinationQueue.Send(queue.ReceiveByLookupId(message.LookupId), destinationQueue.GetTransactionType());
			}
		}

		/// <summary>
		/// Gets a listing of all timeout messages.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeoutInfo> GetTimeoutMessages(MessageQueue queue)
		{
			yield break;
		}

		/// <summary>
		/// Moves the message from the timeout queue to the main queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="messageId">The message id.</param>
		public void MoveTimeoutToMainQueue(MessageQueue queue, string messageId)
		{
			using (var destinationQueue = new MessageQueue(GetTimeoutQueuePath(), QueueAccessMode.Receive))
			{
				var message = destinationQueue.ReceiveById(messageId, queue.GetTransactionType());
				message.AppSpecific = 0;//reset timeout flag
				queue.Send(message, destinationQueue.GetTransactionType());
			}
		}

	    public bool TryMoveMessage(MessageQueue queue, Message message, SubQueue subQueue)
	    {
            using (var destinationQueue = new MessageQueue(queue.Path + "#" + subQueue, QueueAccessMode.Send))
            {
                Message receiveById;
                try
                {
                    receiveById = queue.ReceiveById(message.Id, queue.GetTransactionType());
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                message.AppSpecific = 0;//reset flag
                destinationQueue.Send(receiveById, destinationQueue.GetTransactionType());
                return true;
            }
	    }

	    /// <summary>
		/// Gets the errors queue path.
		/// </summary>
		/// <returns></returns>
		private string GetErrorsQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + errors;
		}

		/// <summary>
		/// Gets the discarded queue path.
		/// </summary>
		/// <returns></returns>
		private string GetDiscardedQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + discarded;
		}

		/// <summary>
		/// Gets the timeout queue path.
		/// </summary>
		/// <returns></returns>
		private string GetTimeoutQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + timeout;
		}

		private string GetSubscriptionQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + subscriptions;
		}
	}

}