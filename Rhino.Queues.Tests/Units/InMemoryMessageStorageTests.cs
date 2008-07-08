using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Queues.Impl;
using Rhino.Queues.Storage.InMemory;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class InMemoryMessageStorageTests
	{
		private InMemoryMessageStorage storage;

		[SetUp]
		public void SetUp()
		{
			storage = new InMemoryMessageStorage(new HashSet<string>
			{
				"http://localhost/test"
			});
		}

		[Test]
		public void Can_add_item()
		{
			storage.Add("http://localhost/test", new TransportMessage());
		}

		[Test]
		public void Can_get_item()
		{
			var message = new TransportMessage();
			storage.Add("http://localhost/test", message);
			var first = storage.PullMessagesFor("http://localhost/test").First();
			Assert.AreEqual(message.Id, first.Id);
		}

		[Test]
		[ExpectedArgumentException("Queue 'http://foo/bar' was not registered")]
		public void Adding_item_to_unregistered_endpoint_throw()
		{
			storage.Add("http://foo/bar", new TransportMessage());
		}


		[Test]
		public void Can_get_item_with_predicate()
		{
			storage.Add("http://localhost/test", new TransportMessage { Message = 1 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 2 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 3 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 4 });

			var count = storage.PullMessagesFor("http://localhost/test", x => ((int) x.Message)%2 == 0).Count();
			Assert.AreEqual(2, count);
		}

		[Test]
		public void Getting_items_with_predicate_remove_from_storage()
		{
			storage.Add("http://localhost/test", new TransportMessage { Message = 1 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 2 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 3 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 4 });

			var count = storage.PullMessagesFor("http://localhost/test", x => ((int)x.Message) % 2 == 0).Count();
			Assert.AreEqual(2, count);
			var exists = storage.PullMessagesFor("http://localhost/test").Any(x => ((int)x.Message) % 2 == 0);
			Assert.IsFalse(exists);
		}

		[Test]
		public void Getting_items_with_predicate_maintain_relative_sort_order()
		{
			storage.Add("http://localhost/test", new TransportMessage { Message = 1 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 2 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 3 });
			storage.Add("http://localhost/test", new TransportMessage { Message = 4 });

			var evenMsgs = storage.PullMessagesFor("http://localhost/test", x => ((int)x.Message) % 2 == 0).ToArray();
			var oddMsgs = storage.PullMessagesFor("http://localhost/test", x => ((int)x.Message) % 2 != 0).ToArray();

			Assert.AreEqual(1, oddMsgs[0].Message);
			Assert.AreEqual(3, oddMsgs[1].Message);


			Assert.AreEqual(2, evenMsgs[0].Message);
			Assert.AreEqual(4, evenMsgs[1].Message);
		}
	}
}
