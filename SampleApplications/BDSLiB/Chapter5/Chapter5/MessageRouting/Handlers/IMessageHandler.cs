namespace Chapter5.MessageRouting.Handlers
{
    public interface IMessageHandler
    {
        string Handle(object msg);
    }
}

