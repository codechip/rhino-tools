using System;

namespace Rhino.ServiceBus
{
    public interface IServiceBus
    {
        /// <summary>
        /// Publish a message to all subscribers.
        /// If there are no subscribers, it will throw.
        /// </summary>
        /// <param name="messages"></param>
        void Publish(params object[] messages);

        /// <summary>
        /// Publish a message to all subscribers.
        /// If there are no subscribers, it ignore the message
        /// </summary>
        /// <param name="messages"></param>
        void Notify(params object[] messages);

        /// <summary>
        /// Reply to the source of the current message
        /// Will throw if not currently handling a message
        /// </summary>
        /// <param name="messages"></param>
        void Reply(params object[] messages);

        /// <summary>
        /// Send the message directly to the specified endpoint
        /// </summary>
        void Send(Uri endpoint, params object[] messages);

        /// <summary>
        /// Get the endpoint of the bus
        /// </summary>
        Uri Endpoint { get; }

        IServiceBus AddInstanceSubscription(IMessageConsumer consumer);
    }
}