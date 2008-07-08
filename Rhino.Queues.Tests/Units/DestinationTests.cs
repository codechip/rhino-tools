using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class DestinationTests
	{
		[Test]
		public void When_passing_full_name_will_parse_to_queue_and_server()
		{
			var destination = new Destination("foo@bar");
			Assert.AreEqual("foo", destination.Queue);
			Assert.AreEqual("bar", destination.Server);
		}

		[Test]
		public void When_passing_relative_queue_will_use_local_server()
		{
			var destination = new Destination("foo","xyz");
			Assert.AreEqual("foo", destination.Queue);
			Assert.AreEqual("xyz", destination.Server);
		}

		[Test]
		public void When_passing_absolute_queue_will_and_local_server_will_use_specified_queue()
		{
			var destination = new Destination("foo@xyz", "bar");
			Assert.AreEqual("foo", destination.Queue);
			Assert.AreEqual("xyz", destination.Server);
		}

		[Test]
		[ExpectedArgumentException]
		public void When_passing_relative_queue_without_local_server_will_thorw()
		{
			new Destination("foo");
		}
	}
}