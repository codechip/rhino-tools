using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueListenerTests
	{
		private QueueListener listener;
		private IQueueFactoryImpl stubbedQueueFactory;
		private IQueueImpl stubbedQueue;

		[SetUp]
		public void Setup()
		{
			stubbedQueue = MockRepository.GenerateStub<IQueueImpl>();
			stubbedQueueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			listener = new QueueListener(new Uri("queue://localhost/testQueue"));
		}

		[TearDown]
		public void Teardown()
		{
			listener.Stop();
		}

		[Test]
		public void When_getting_valid_request_will_call_queue()
		{
			listener.Start(stubbedQueueFactory);

			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Source = new Uri("queue://localhost/myTester"),
				Destination = new Uri("queue://localhost/testQueue"),
				Messages = new[]
				{
					new QueueMessage(),
				}
			};
			// output queue
			stubbedQueueFactory.Stub(x => x.GetLocalQueue(Arg.Is(batch.DestinationQueue)))
				.Return(stubbedQueue)
				.Repeat.Any();
			// input queue
			stubbedQueueFactory.Stub(x => x.GetLocalQueue(Arg.Is(batch.SourceQueue)))
				.Return(MockRepository.GenerateStub<IQueueImpl>())
				.Repeat.Any();


			var cmd = new SendToRemoteServerCommand(stubbedQueueFactory, batch);
			var waitForSendToEnd = new ManualResetEvent(false);
			cmd.Done += () => waitForSendToEnd.Set();

			cmd.Execute();

			waitForSendToEnd.WaitOne();

			IList<object[]> madeOn = stubbedQueue
				.GetArgumentsForCallsMadeOn(x => x.AcceptMessages(Arg<QueueMessage[]>.Is.Anything));
			var msg = (QueueMessage[])madeOn[0][0];
			Assert.AreEqual(msg[0].Id, batch.Messages[0].Id);
		}

		[Test]
		public void When_getting_request_for_invalid_queue_will_return_404()
		{
			listener.Start(stubbedQueueFactory);

			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Source = new Uri("queue://localhost/myTester"),
				Destination = new Uri("queue://localhost/testQueue"),
				Messages = new[]
				{
					new QueueMessage(),
				}
			};

			// output queue doesn't exists

			// input queue
			stubbedQueueFactory.Stub(x => x.GetLocalQueue(batch.SourceQueue))
				.Return(stubbedQueue)
				.Repeat.Any();


			var cmd = new SendToRemoteServerCommand(stubbedQueueFactory, batch);
			var waitForSendToEnd = new ManualResetEvent(false);
			cmd.Done += () => waitForSendToEnd.Set();

			WebException e = null;
			stubbedQueue.Stub(x => x.FailedToTransfer(null, null))
				.IgnoreArguments()
				.Do(invocation => e = (WebException) invocation.Arguments[1]);

			cmd.Execute();

			waitForSendToEnd.WaitOne();

			Assert.AreEqual(HttpStatusCode.NotFound,
				((HttpWebResponse)e.Response).StatusCode);
			
		}
	}
}