namespace Rhino.ServiceBus.Internal
{
    public class DesiredMessageActionFromTransport
    {
        public MessageAction MessageAction { get; set; }
        public string Destination { get; set; }
    }
}