using System;

namespace Rhino.Queues
{
	public interface IMessageQueue : IDisposable
	{
		Message Send(object msg);
		Message Recieve();
	}

}