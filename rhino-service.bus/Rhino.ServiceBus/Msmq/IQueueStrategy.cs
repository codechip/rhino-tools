using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
	using System.Collections.Generic;

	/// <summary>
    /// Encapsulates handling of messages based on queue layout
    /// </summary>
    public interface IQueueStrategy
    {
		MessageQueue InitializeQueue(Uri endpoint);
        
        /// <summary>
        /// Creates the subscription queue URI.
        /// </summary>
        /// <param name="subscriptionQueue">The subscription queue.</param>
        /// <returns></returns>
        Uri CreateSubscriptionQueueUri(Uri subscriptionQueue);
        
		/// <summary>
		/// Moves the <paramref name="message"/> to the timeout queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
    	void MoveToTimeoutQueue(MessageQueue queue, Message message);

		/// <summary>
		/// Gets a listing of all timeout messages.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TimeoutInfo> GetTimeoutMessages(MessageQueue queue);

		/// <summary>
		/// Moves the message from the timeout queue to the main queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="messageId">The message id.</param>
		void MoveTimeoutToMainQueue(MessageQueue queue, string messageId);

        bool TryMoveMessage(MessageQueue queue, Message message, SubQueue subQueue, out string msgId);
    }
}
