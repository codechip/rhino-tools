namespace BDSLiB.MessageRouting.Handlers
{
    using Messages;

    public abstract class AbstractHandler<TMessage> : IMessageHandler
    {
        string IMessageHandler.Handle(object msg)
        {
            return Handle((TMessage) msg);
        }

        public abstract string Handle(TMessage message);
    }
}