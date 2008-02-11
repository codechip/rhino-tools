namespace BDSLiB.MessageRouting.DSL
{
    using System;
    using Boo.Lang;
    using Handlers;

    /// <summary>
    /// This delegate is used by the DSL to return the 
    /// internal representation of the message
    /// </summary>
    public delegate object MessageTransformer();

    /// <summary>
    /// Base class for the Routing DSL
    /// </summary>
    public abstract class RoutingBase
    {
        /// <summary>
        /// The current message, using IQuackFu
        /// to abstract the message implementation
        /// </summary>
        protected IQuackFu msg;

        /// <summary>
        /// The result of routing this message, if the DSL decided
        /// it was routable by this DSL instance.
        /// </summary>
        public string Result;

        /// <summary>
        /// Initializes this instance with the specified message
        /// </summary>
        /// <param name="message">The message.</param>
        public void Initialize(IQuackFu message)
        {
            msg = message;
        }

        /// <summary>
        /// Routes the current message. This method is overriden by the 
        /// DSL. This is also where the logic of the DSL executes.
        /// </summary>
        public abstract void Route();

        /// <summary>
        /// Handles the message with the specified handler type
        /// and the message transformer.
        /// </summary>
        /// <param name="handlerType">The type.</param>
        /// <param name="transformer">The transformer.</param>
        public void HandleWith(Type handlerType, MessageTransformer transformer)
        {
            IMessageHandler handler = 
                (IMessageHandler) Activator.CreateInstance(handlerType);
            Result = handler.Handle(transformer());
        }
    }
}