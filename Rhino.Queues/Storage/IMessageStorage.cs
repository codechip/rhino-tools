using System;
using System.Collections.Generic;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Storage
{
	public interface IMessageStorage : IDisposable
	{
		void Add(string name, TransportMessage message);
		IEnumerable<TransportMessage> GetMessagesFor(string name);
		bool WaitForNewMessages(string name);
		bool Exists(string name);
		string WaitForNewMessages();
	}
}