using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
    /// <summary>
    /// Encapsulates handling of messages based on queue layout
    /// </summary>
    public interface IQueueStrategy
    {
        /// <summary>
        /// Creates the subscription queue URI.
        /// </summary>
        /// <param name="subscriptionQueue">The subscription queue.</param>
        /// <returns></returns>
        Uri CreateSubscriptionQueueUri(Uri subscriptionQueue);
        /// <summary>
        /// Moves the <paramref name="message"/> to subscription queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        void MoveToSubscriptionQueue(MessageQueue queue, Message message);
        /// <summary>
        /// Moves the <paramref name="message"/> to errors queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        void MoveToErrorsQueue(MessageQueue queue, Message message);
        /// <summary>
        /// Moves the <paramref name="message"/> to discarded queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        void MoveToDiscardedQueue(MessageQueue queue, Message message);
    }
}
