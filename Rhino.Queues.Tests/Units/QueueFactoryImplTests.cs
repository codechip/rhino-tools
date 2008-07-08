using System;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;
using Rhino.Queues.Network;

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
			var listenerFactory = MockRepository.GenerateStub<IListenerFactory>();
			listenerFactory.Stub(x => x.Create(null, null)).IgnoreArguments().Return(
				MockRepository.GenerateStub<IListener>());
			var senderFactory = MockRepository.GenerateStub<ISenderFactory>();
			senderFactory.Stub(x => x.Create(null)).IgnoreArguments().Return(
				MockRepository.GenerateStub<ISender>());

			messageStorageFactory = new TestMessageStorageFactory();
			factory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
			{
				{"foo", "http://localhost/foo/"},
				{"bar", "http://localhost/self/"}
			}, new[] { "foo" }, listenerFactory, senderFactory);
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
			factory.OpenQueue("test@blah").Send("foo");
		}

		[Test]
		[ExpectedArgumentException("Destination cannot be null or empty")]
		public void When_sending_to_an_empty_destination_will_throw()
		{
			factory.OpenQueue("").Send("foo");
		}

		[Test]
		[ExpectedArgumentException("Destination cannot be null or empty")]
		public void When_sending_to_an_null_destination_will_throw()
		{
			factory.OpenQueue(null).Send("foo");
		}


		[Test]
		[ExpectedArgumentNullException]
		public void When_sending_null_message_will_throw()
		{
			factory.OpenQueue("foo").Send(null);
		}

		[Test]
		[ExpectedArgumentException("Message QueueFactoryImplTests must be serializable")]
		public void When_sending_unserializable_message_will_throw()
		{
			factory.OpenQueue("foo").Send(this);
		}


		[Test]
		public void When_sending_message_will_place_in_storage()
		{
			factory.OpenQueue("test@foo").Send("my msg2");
			Assert.AreEqual("my msg2",
							messageStorageFactory.OutgoingStorage
								.PullMessagesFor("http://localhost/foo/").First()
									.Message.Value);
		}

		[Test]
		public void When_recieving_message_will_get_it_from_storage()
		{
			messageStorageFactory.IncomingStorage.Add("foo", new TransportMessage { Message = new Message { Value = "my msg 5" } });
			var recieve = factory.OpenQueue("foo").Recieve();
			Assert.AreEqual("my msg 5", recieve.Value);
		}

		[Test]
		public void Can_send_message_to_local_without_specifying_destination()
		{
			factory.OpenQueue("test").Send("my msg1");
			Assert.AreEqual("my msg1",
							messageStorageFactory.OutgoingStorage
								.PullMessagesFor("http://localhost/self/").First()
									.Message.Value);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Cannot send or queue before factory 'bar' is started")]
		public void Cannot_call_send_without_calling_start()
		{
			var myFactory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
            {
            	{"foo", "http://localhost/foo/"},
            	{"bar", "http://localhost/self2/"}
            }, new[] { "foo" }, new ListenerFactory(1), new SenderFactory(1));
			myFactory.OpenQueue("foo").Send(1);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Cannot send or queue before factory 'bar' is started")]
		public void Cannot_call_queue_without_calling_start()
		{
			var myFactory = new QueueFactoryImpl("bar", messageStorageFactory, new Dictionary<string, string>
			{
				{"foo", "http://localhost/foo/"},
				{"bar", "http://localhost/self3/"}
			}, new[] { "foo" }, new ListenerFactory(1), new SenderFactory(1));
			myFactory.OpenQueue("foo");
		}
	}
}