namespace Chapter5.MessageRouting.DSL
{
    using System;
    using Boo.Lang;
    using Handlers;

    public abstract class RoutingBase
    {
        public string Result;

        #region Delegates

        public delegate object MessageTransformer();

        #endregion

        protected IQuackFu msg;

        public void Initialize(IQuackFu message)
        {
            msg = message;
        }

        public abstract void Route();

        public void HandleWith(Type type, MessageTransformer transformer)
        {
            IMessageHandler handler = (IMessageHandler) Activator.CreateInstance(type);
            Result = handler.Handle(transformer());
        }
    }
}