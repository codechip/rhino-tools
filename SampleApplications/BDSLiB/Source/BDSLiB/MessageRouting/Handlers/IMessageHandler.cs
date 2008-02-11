namespace BDSLiB.MessageRouting.Handlers
{
    public interface IMessageHandler
    {
        string Handle(object msg);
    }
}

