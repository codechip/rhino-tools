using System;
using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Commands;
using Rhino.Queues.Data;
using Rhino.Queues.Impl;
using Rhino.Queues.Workers;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueWorkerTests
	{
		private IOutgoingMessageRepository stubbedOutgoingMessageRepository;
		private ICommandFactory stubbedCommandFactory;
		private IQueueFactoryImpl stubbedQueueFactory;

		[SetUp]
		public void Setup()
		{
			stubbedCommandFactory = MockRepository.GenerateStub<ICommandFactory>();
			stubbedQueueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
		}

		[Test]
		public void For_each_batch_from_repository_will_create_and_execute_command()
		{
			var stubbedCommand = MockRepository.GenerateMock<ICommand>();
			var queueWorker = new QueueWorker(stubbedCommandFactory, stubbedQueueFactory, stubbedOutgoingMessageRepository);

			stubbedCommandFactory
				.Stub(x => x.CreateCommand(Arg<SingleDestinationMessageBatch>.Is.Anything))
				.Return(stubbedCommand)
				.Repeat.Any();

			stubbedCommand.Expect(x => x.Execute()).Repeat.Times(3);

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch
				{
					DestinationBatches = new[]
					{
						new SingleDestinationMessageBatch(),
						new SingleDestinationMessageBatch(),
						new SingleDestinationMessageBatch(),
					}
				});

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Do(delegate { queueWorker.Stop(); });

			queueWorker.Run();

			stubbedCommand.VerifyAllExpectations();
		}

		[Test]
		public void For_each_batch_from_repository_will_set_source_to_queue_local_uri()
		{
			var stubbedCommand = MockRepository.GenerateMock<ICommand>();
			var queueWorker = new QueueWorker(stubbedCommandFactory, stubbedQueueFactory, stubbedOutgoingMessageRepository);

			stubbedQueueFactory
				.Stub(x => x.LocalUri)
				.Return(new Uri("queue://foo/bar"))
				.Repeat.Any();

			stubbedCommandFactory
				.Stub(x => x.CreateCommand(Arg<SingleDestinationMessageBatch>.Is.Anything))
				.Return(stubbedCommand)
				.Repeat.Any();

			var batches = new[]
			{
				new SingleDestinationMessageBatch(),
				new SingleDestinationMessageBatch(),
				new SingleDestinationMessageBatch(),
			};
			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch
				{
					DestinationBatches = batches
				});

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Do(delegate { queueWorker.Stop(); });

			queueWorker.Run();

			foreach (var batch in batches)
			{
				Assert.AreEqual(new Uri("queue://foo/bar"), batch.Source);
			}
		}

		[Test]
		public void For_each_message_in_all_batches_from_repository_will_set_source_to_queue_local_uri()
		{
			var stubbedCommand = MockRepository.GenerateMock<ICommand>();
			var queueWorker = new QueueWorker(stubbedCommandFactory, stubbedQueueFactory, stubbedOutgoingMessageRepository);

			stubbedQueueFactory
				.Stub(x => x.LocalUri)
				.Return(new Uri("queue://foo/bar"))
				.Repeat.Any();

			stubbedCommandFactory
				.Stub(x => x.CreateCommand(Arg<SingleDestinationMessageBatch>.Is.Anything))
				.Return(stubbedCommand)
				.Repeat.Any();

			var batches = new[]
			{
				new SingleDestinationMessageBatch{Messages = new []{new QueueMessage(), }},
				new SingleDestinationMessageBatch{Messages = new []{new QueueMessage(), new QueueMessage(), }},
				new SingleDestinationMessageBatch{Messages = new []{new QueueMessage(), new QueueMessage(), new QueueMessage(), }},
			};
			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch
				{
					DestinationBatches = batches
				});

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Do(delegate { queueWorker.Stop(); });

			queueWorker.Run();

			foreach (var batch in batches)
			{
				foreach (var message in batch.Messages)
				{
					Assert.AreEqual(new Uri("queue://foo/bar"), message.Source);
				}
			}
		}

		[Test]
		public void When_message_repository_has_no_message_it_will_wait_for_timeout_or_notification()
		{
			var queueWorker = new QueueWorker(stubbedCommandFactory, stubbedQueueFactory, stubbedOutgoingMessageRepository);

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Repeat.Any();

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Do(delegate { queueWorker.Stop(); });
			
			var running = 0;
			var waiting = 0;
			
			queueWorker.Running += () => running++;
			queueWorker.Waiting += () =>
			{
				waiting++;
				if (waiting > 1)
				{
					// this also cancel any future waiting
					queueWorker.Stop();
				}
			};
			queueWorker.TimeToWait = () => TimeSpan.FromMilliseconds(50);
			DateTime before = DateTime.Now;
			queueWorker.Run();

			Assert.AreEqual(2, running);
			Assert.AreEqual(2, waiting);

			DateTime after = DateTime.Now;
			Console.WriteLine(after - before);
			Assert.IsTrue((after - before).TotalMilliseconds > 50);
		}

		[Test]
		public void When_waiting_can_be_woken_up_using_new_message_stored()
		{
			var queueWorker = new QueueWorker(stubbedCommandFactory, stubbedQueueFactory, stubbedOutgoingMessageRepository);

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Repeat.Any();

			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Do(delegate { queueWorker.Stop(); });

			var running = 0;
			var waiting = 0;

			queueWorker.Running += () => running++;
			queueWorker.Waiting += () =>
			{
				waiting++;
				if (waiting == 1)
				{
					ThreadPool.QueueUserWorkItem(state =>
					{
						Thread.Sleep(50);
						queueWorker.NewMessageStored();
					});
				}
				else if( waiting==2)
				{
					queueWorker.Stop();
				}
			};
			queueWorker.TimeToWait = () => TimeSpan.FromDays(1);

			queueWorker.Run();

			Assert.AreEqual(2, running);
			Assert.AreEqual(2, waiting);
		}
	}
}