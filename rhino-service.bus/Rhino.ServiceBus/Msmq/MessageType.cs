namespace Rhino.ServiceBus.Msmq
{
	public enum MessageType
	{
		StandardMessage = 0,
		DiscardedMessageMarker = 0xD13574,
		ErrorDescriptionMessageMarker = 0xE7707,
		AdministrativeMessageMarker = 42,
		TimeoutMessageMarker = 0x1e023abc,
        LoadBalancerMessage = 0xBA19CA7
	}
}