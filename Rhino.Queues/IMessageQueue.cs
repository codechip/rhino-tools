using System;
using Rhino.Queues.Impl;

namespace Rhino.Queues
{
	public interface IMessageQueue : IDisposable
	{
		void Send(object msg);
		object Recieve();
	}

}