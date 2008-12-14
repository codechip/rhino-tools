namespace Rhino.ServiceBus.Messages
{
    public class RemoveSubscription
    {
        public string Type { get; set; }
        public string Endpoint { get; set; }
    }
}