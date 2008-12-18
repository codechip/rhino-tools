using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
    /// <summary>
    /// Default subqueue stragey enabled in MSMQ 4.0
    /// </summary>
    public class SubQueueStrategy : IQueueStrategy
    {
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
        /// Moves the <paramref name="message"/> to subscription queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        public void MoveToSubscriptionQueue(MessageQueue queue, Message message)
        {
            queue.MoveToSubQueue("subscriptions", message);
        }

        /// <summary>
        /// Moves the <paramref name="message"/> to errors queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        public void MoveToErrorsQueue(MessageQueue queue, Message message)
        {
            queue.MoveToSubQueue("errors", message);
        }

        /// <summary>
        /// Moves the <paramref name="message"/> to discarded queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        public void MoveToDiscardedQueue(MessageQueue queue, Message message)
        {
            queue.MoveToSubQueue("discarded", message);
        }
    }
}