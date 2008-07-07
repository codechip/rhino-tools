using System;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class QueueFactoryImplTests
	{
		private IQueueFactory factory;
		private TestMessageStorageFactory messageStorageFactory;

		[SetUp]
		public void SetUp()
		{
			messageStorageFactory = new TestMessageStorageFactory();
			factory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
			{
				{"foo", "http://localhost/foo/"},
				{"bar", "http://localhost/self/"}
			}, new[] {"foo"}, 0, 0);
			factory.Start();
		}

		[TearDown]
		public void TearDown()
		{
			factory.Dispose();
		}

		[Test]
		[ExpectedArgumentException(
			"Destination 'blah' endpoint was not registered. Did you forget to call Map('blah').To('http://some/end/point');")]
		public void When_sending_to_an_unknown_destination_will_throw()
		{
			factory.Send("test@blah", "foo");
		}

		[Test]
		[ExpectedArgumentException("Destination cannot be null or empty")]
		public void When_sending_to_an_empty_destination_will_throw()
		{
			factory.Send("", "foo");
		}

		[Test]
		[ExpectedArgumentException("Destination cannot be null or empty")]
		public void When_sending_to_an_null_destination_will_throw()
		{
			factory.Send(null, "foo");
		}


		[Test]
		[ExpectedArgumentNullException]
		public void When_sending_null_message_will_throw()
		{
			factory.Send("foo", null);
		}

		[Test]
		[ExpectedArgumentException("Message QueueFactoryImplTests must be serializable")]
		public void When_sending_unserializable_message_will_throw()
		{
			factory.Send("foo", this);
		}


		[Test]
		public void When_sending_message_will_place_in_storage()
		{
			factory.Send("test@foo", "my msg2");
			Assert.AreEqual("my msg2",
			                messageStorageFactory.OutgoingStorage.GetMessagesFor("http://localhost/foo/").First().Message);
		}

		[Test]
		public void When_recieving_message_will_get_it_from_storage()
		{
			messageStorageFactory.IncomingStorage.Add("foo", new TransportMessage{Message = "my msg 5"});
			var recieve = factory.Queue("foo").Recieve();
			Assert.AreEqual("my msg 5", recieve);
		}

		[Test]
		public void Can_send_message_to_local_without_specifying_destination()
		{
			factory.Send("test", "my msg1");
			Assert.AreEqual("my msg1",
			                messageStorageFactory.OutgoingStorage.GetMessagesFor("http://localhost/self/").First().Message);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Cannot send or queue before factory 'bar' is started")]
		public void Cannot_call_send_without_calling_start()
		{
            var myFactory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
			{
				{"foo", "http://localhost/foo/"},
				{"bar", "http://localhost/self2/"}
			}, new[] { "foo" }, 3, 3);
			myFactory.Send("foo", 1);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Cannot send or queue before factory 'bar' is started")]
		public void Cannot_call_queue_without_calling_start()
		{
			var myFactory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
			{
				{"foo", "http://localhost/foo/"},
				{"bar", "http://localhost/self3/"}
			}, new[] { "foo" }, 3, 3);
			myFactory.Queue("foo");
		}
	}
}