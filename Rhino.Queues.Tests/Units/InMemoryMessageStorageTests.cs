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
			var first = storage.GetMessagesFor("http://localhost/test").First();
			Assert.AreEqual(message.Id, first.Id);
		}

		[Test]
		[ExpectedArgumentException("Queue 'http://foo/bar' was not registered")]
		public void Adding_item_to_unregistered_endpoint_throw()
		{
			storage.Add("http://foo/bar", new TransportMessage());
		}
	}
}