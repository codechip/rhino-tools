namespace Rhino.Queues.Storage.InMemory
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using Impl;
	using Threading;

	public class QueuePackage
	{
		private readonly LinkedList<TransportMessage> messages = new LinkedList<TransportMessage>();
		private readonly IBlockingQueue<object> events = new BlockingQueue<object>();

		public IEnumerable<TransportMessage> MessagesInReverseOrder
		{
			get
			{
				return new ReverseEnumerable(messages);
			}
		}

		public class ReverseEnumerable : IEnumerable<TransportMessage>
		{
			private readonly LinkedList<TransportMessage> messages;
			private LinkedListNode<TransportMessage> currentItemToRemove;

			public ReverseEnumerable(LinkedList<TransportMessage> messages)
			{
				this.messages = messages;
			}

			public IEnumerator<TransportMessage> GetEnumerator()
			{
				var current = messages.Last;
				while (current != null)
				{
					var value = current.Value;
					currentItemToRemove = current;
					current = current.Previous;
					yield return value;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void RemoveCurrent()
			{
				if (currentItemToRemove != null)
				{
					messages.Remove(currentItemToRemove);
					currentItemToRemove = null;
				}
			}
		}

		public bool WaitForNewMessage()
		{
			object ignored;
			return events.Dequeue(TimeSpan.FromMilliseconds(Timeout.Infinite), out ignored);
		}

		public void Add(TransportMessage message)
		{
			messages.AddFirst(message);
			events.Enqueue(new object());
		}

	}
}