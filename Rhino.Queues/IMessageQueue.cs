using System;
using Rhino.Queues.Impl;

namespace Rhino.Queues
{
	public interface IMessageQueue
	{
		object Recieve();
		void PutAll(TransportMessage[] msgs);
	}
}