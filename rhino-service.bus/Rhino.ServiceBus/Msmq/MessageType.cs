namespace Rhino.ServiceBus.Msmq
{
	public enum MessageType
	{
		StandardMessage = 0,
		DiscardedMessageMarker = 1,
		ErrorDescriptionMessageMarker = 2,
		ShutDownMessageMarker = 3,
		AdministrativeMessageMarker = 4,
		TimeoutMessageMarker = 5,
	    LoadBalancerMessage = 6
	}
}