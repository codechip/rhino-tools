using System;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Cfg;
using Rhino.Queues.Impl;
using Rhino.Queues.Network;
using Rhino.Queues.Storage;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class QueueFactory_error_handling_tests
	{
		private QueueFactoryImpl serverFactory;

		[SetUp]
		public void SetUp()
		{
			SystemTime.Now = () => new DateTime(2000, 1, 1);
			serverFactory = new Configuration("server")
			                	.Map("server").To("http://localhost:9999/server/")
			                	.Map("client").To("http://localhost:9999/client/")
			                	.RegisterQueue("foo")
			                	.ListenerThreads(1)
			                	.SenderThreads(1)
			                	.BuildQueueFactory() as QueueFactoryImpl;
		}

		[TearDown]
		public void TearDown()
		{
			SystemTime.Now = () => DateTime.Now;
		}

		[Test]
		public void When_error_occured_will_put_messages_back_in_their_queue()
		{
			serverFactory.OnSendError(new Exception(), new[]
			{
				new TransportMessage
				{
					Destination = new Destination {Queue = "foo", Server = "client"},
					Message = 5,
					SendAt = SystemTime.Now()
				}
			});
			var message = serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").First();
			Assert.AreEqual(5, message.Message);
		}


		[Test]
		public void When_error_occured_and_failure_count_over_500_will_NOT_add_back_to_queue()
		{
			serverFactory.OnSendError(new Exception(), new[]
			{
				new TransportMessage
				{
					Destination = new Destination {Queue = "foo", Server = "client"},
					Message = 5,
					SendAt = SystemTime.Now(),
					FailureCount = 500
				}
			});
			var message = serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault();
			Assert.IsNull(message);
		}

		[Test]
		public void When_error_occured_and_failure_count_over_500_will_raise_final_failure_event()
		{
			TransportMessage failedMsg = null;
			Exception ex = null;
			serverFactory.FinalDeliveryFailure += delegate(TransportMessage msg, Exception e)
			{
				failedMsg = msg;
				ex = e;
			};
			serverFactory.OnSendError(new Exception("error"), new[]
			{
				new TransportMessage
				{
					Destination = new Destination {Queue = "foo", Server = "client"},
					Message = 5,
					SendAt = SystemTime.Now(),
					FailureCount = 500
				}
			});
			Assert.AreEqual(5, failedMsg.Message);
			Assert.AreEqual("error", ex.Message);
		}

		[Test]
		public void When_error_occured_will_increase_failure_count()
		{
			var msg = new TransportMessage
			{
				Destination = new Destination {Queue = "foo", Server = "client"},
				Message = 5,
				SendAt = SystemTime.Now()
			};
			serverFactory.OnSendError(new Exception(), new[]
			{
				msg
			});
			Assert.AreEqual(1, msg.FailureCount);
		}

		[Test]
		public void When_error_occured_will_increase_send_at_time()
		{
			var msg = new TransportMessage
			{
				Destination = new Destination {Queue = "foo", Server = "client"},
				Message = 5,
				SendAt = SystemTime.Now()
			};
			serverFactory.OnSendError(new Exception(), new[]
			{
				msg
			});
			Assert.AreEqual(SystemTime.Now().AddSeconds(2), msg.SendAt);
		}

		[Test]
		public void When_error_occured_will_increase_send_at_time_in_increasing_rate()
		{
			var msg = new TransportMessage
			{
				Destination = new Destination {Queue = "foo", Server = "client"},
				Message = 5,
				SendAt = SystemTime.Now()
			};
			serverFactory.OnSendError(new Exception(), new[] {msg});
			Assert.AreEqual(SystemTime.Now().AddSeconds(2), msg.SendAt);
			serverFactory.OnSendError(new Exception(), new[] {msg});
			Assert.AreEqual(SystemTime.Now().AddSeconds(4), msg.SendAt);
			serverFactory.OnSendError(new Exception(), new[] {msg});
			Assert.AreEqual(SystemTime.Now().AddSeconds(6), msg.SendAt);
			serverFactory.OnSendError(new Exception(), new[] {msg});
			Assert.AreEqual(SystemTime.Now().AddSeconds(8), msg.SendAt);
		}

		[Test]
		public void Will_register_to_sender_error_event()
		{
			var storageFactory = MockRepository.GenerateStub<IStorageFactory>();
			var mapping = new Dictionary<string, string> {{"test", "http://localhost/test/"}};
			var queues = new[] {"test"};
			var senderFactory = MockRepository.GenerateStub<ISenderFactory>();
			var sender = MockRepository.GenerateStub<ISender>();
			storageFactory.Stub(x => x.ForIncomingMessages(null)).IgnoreArguments().Return(
				MockRepository.GenerateStub<IMessageStorage>());
			storageFactory.Stub(x => x.ForOutgoingMessages(null)).IgnoreArguments().Return(
				MockRepository.GenerateStub<IMessageStorage>());

			senderFactory.Stub(x => x.Create(null)).IgnoreArguments().Return(sender);

			using (var queueFactoryImpl = new QueueFactoryImpl("test", storageFactory, mapping, queues,
											new ListenerFactory(1),
											senderFactory))
			{
				queueFactoryImpl.Start();

				sender.AssertWasCalled(x => x.Error += queueFactoryImpl.OnSendError);
			}

		}
	}
}