using System;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Network
{
	public interface ISender : IDisposable
	{
		void Start();
		event Action<Exception, TransportMessage, MessageSendFailure> Error;
	}
}