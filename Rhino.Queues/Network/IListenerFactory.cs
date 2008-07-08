using System;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Network
{
	public interface IListenerFactory : IDisposable
	{
		IListener Create(IQueueFactoryImpl queueFactory, string endpoint);
	}
}