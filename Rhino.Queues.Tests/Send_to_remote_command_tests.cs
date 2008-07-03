using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Send_to_remote_command_tests
	{
		private HttpListener listener;
		private IQueueImpl stubbedQueue;
		private IQueueFactoryImpl stubbedQueueFactoryImpl;

		[SetUp]
		public void Setup()
		{
			stubbedQueue = MockRepository.GenerateStub<IQueueImpl>();
			stubbedQueueFactoryImpl = MockRepository.GenerateStub<IQueueFactoryImpl>();
			stubbedQueueFactoryImpl
				.Stub(x => x.GetLocalQueue("testQueue"))
				.Return(stubbedQueue)
				.Repeat.Any();

			listener = new HttpListener();
			listener.Prefixes.Add("http://localhost:9523/testQueue/");
			listener.Start();
		}

		[TearDown]
		public void Teardown()
		{
			listener.Stop();
		}

		[Test]
		public void When_recieve_a_batch_to_send_will_send_to_remote_server()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Source = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, }
			};

			var cmd = new SendToRemoteServerCommand(stubbedQueueFactoryImpl, destinationBatch);
			ThreadPool.QueueUserWorkItem(state =>
			{
				cmd.Execute();
			});

			QueueMessage[] sentMsgs = GetSentMsgs(destinationBatch);

			Assert.AreEqual(msg.Id, sentMsgs[0].Id);
		}

		[Test]
		public void When_recieve_a_batch_to_send_will_send_to_remote_server_with_several_messages_in_same_call()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Source = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, new QueueMessage(), new QueueMessage(), }
			};

			var cmd = new SendToRemoteServerCommand(stubbedQueueFactoryImpl, destinationBatch);
			ThreadPool.QueueUserWorkItem(state =>
			{
				cmd.Execute();
			});

			QueueMessage[] sentMsgs = GetSentMsgs(destinationBatch);

			Assert.AreEqual(3, sentMsgs.Length);
		}

		[Test]
		public void When_successfully_sent_batch_will_notify_queue()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Source = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, }
			};

			var cmd = new SendToRemoteServerCommand(stubbedQueueFactoryImpl, destinationBatch);
			var waitForCompleteion = new ManualResetEvent(false);
			cmd.Done += () => waitForCompleteion.Set();
			ThreadPool.QueueUserWorkItem(state =>
			{
				cmd.Execute();
			});

			GetSentMsgs(destinationBatch);
			waitForCompleteion.WaitOne();
			stubbedQueue.AssertWasCalled(x => x.SuccessfullyTransfered(destinationBatch));
		}

		[Test]
		public void When_failed_to_send_batch_will_notify_queue()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Source = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, }
			};

			var cmd = new SendToRemoteServerCommand(stubbedQueueFactoryImpl, destinationBatch);
			var waitForCompleteion = new ManualResetEvent(false);
			cmd.Done += () => waitForCompleteion.Set();
			ThreadPool.QueueUserWorkItem(state =>
			{
				cmd.Execute();
			});

			var context = listener.GetContext();
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			context.Response.Close();

			waitForCompleteion.WaitOne();
			stubbedQueue.AssertWasCalled(x => x.FailedToTransfer(
				Arg<SingleDestinationMessageBatch>.Is.Equal(destinationBatch), 
				Arg<Exception>.Is.Anything));
		}

		private QueueMessage[] GetSentMsgs(SingleDestinationMessageBatch destinationBatch)
		{
			var context = listener.GetContext();
			Assert.AreEqual("PUT", context.Request.HttpMethod);
			Assert.AreEqual(
				destinationBatch.BatchId,
				new Guid(context.Request.Headers["batch-id"])
				);

			var sentMsgs = (SingleDestinationMessageBatch)new BinaryFormatter().Deserialize(context.Request.InputStream);
			using (var response = new StreamWriter(context.Response.OutputStream))
			{
				response.Write("Got it!");
			}
			return sentMsgs.Messages;
		}
	}
}