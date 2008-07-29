using System;
using System.Collections.Generic;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Storage
{
	public interface IMessageStorage : IDisposable
	{
		void Add(string name, TransportMessage message);
		
		IEnumerable<TransportMessage> PullMessagesFor(string name, 
			Predicate<TransportMessage> predicate);

		IEnumerable<TransportMessage> PullMessagesFor(string name);
		
		bool WaitForNewMessages(string name);
		
		bool Exists(string name);

		bool WaitForNewMessages(TimeSpan timeToWait, out string queueWithNewMessages);

		IEnumerable<string> Queues { get;  }
	}
}