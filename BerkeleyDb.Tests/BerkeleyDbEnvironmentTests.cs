using System;
using System.IO;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class BerkeleyDbEnvironmentTests
	{
		[SetUp]
		public void Setup()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
				environment.DeleteQueue("my-queue");
		}

		[Test]
		public void Environment_is_transactional()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
				Assert.IsTrue(environment.IsTransactional);
		}

		[Test]
		public void Environment_flags_are_transactional_logging_recoverable_create_lock_mempool()
		{
			var expected = Env.OpenFlags.Create |
			               Env.OpenFlags.InitLock |
			               Env.OpenFlags.InitLog |
                           Env.OpenFlags.InitMPool |
                           Env.OpenFlags.Register |
                           Env.OpenFlags.ThreadSafe |
						   Env.OpenFlags.InitTxn |
						   Env.OpenFlags.Recover;
			Assert.AreEqual(expected, BerkeleyDbEnvironment.CreationFlags);
		}

		[Test]
		public void Can_start_transactions_from_environment()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
				Assert.IsNotNull(tx);
		}

		[Test]
		public void Can_commit_transactions_from_environment()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
				tx.Commit();
		}

		[Test]
		public void Can_rollback_transactions_from_environment()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
				tx.Commit();
		}

		[Test]
		public void Can_nest_transactions()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tx = environment.BeginTransaction())
				{
					Assert.AreSame(tx, environment.CurrentTransaction);
					using (var child = environment.BeginTransaction())
					{
						Assert.AreSame(child, environment.CurrentTransaction);
						child.Commit();	
					}
					Assert.AreSame(tx, environment.CurrentTransaction);
					tx.Commit();
				}
			}
		}

		[Test]
		public void Can_verify_that_queue_does_not_exists()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				Assert.IsFalse(
					environment.DoesQueueExists("my-queue")
					);
			}
		}
		
		[Test]
		public void Can_create_queue_from_environment()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (environment.BeginTransaction())
			{
				environment.CreateQueue("my-queue", 100);
				Assert.IsTrue(
				    environment.DoesQueueExists("my-queue")
				    );
			}
		}

		[Test]
		public void Can_open_queue_from_environment()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (environment.BeginTransaction())
			{
				environment.CreateQueue("my-queue", 100);
				var queue = environment.OpenQueue("my-queue");
				Assert.IsNotNull(queue);
			}
		}

		[Test]
		[ExpectedException(typeof(QueueDoesNotExistsException))]
		public void Trying_to_open_queue_that_does_not_exists_should_thorw()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (environment.BeginTransaction())
			{
				environment.OpenQueue("my-queue");
			}
		}

		[Test]
		public void Environments_instances_should_all_use_the_same_underlying_bdb_environment_for_same_name()
		{
			using (var environment1 = new BerkeleyDbEnvironment("test"))
			using (var environment2 = new BerkeleyDbEnvironment("test"))
			{
				Assert.AreSame(environment1.Inner, environment2.Inner);
			}	
		}

		[Test]
		public void Different_env_names_should_have_different_underlying_instances()
		{
			using (var environment1 = new BerkeleyDbEnvironment("test"))
			using (var environment2 = new BerkeleyDbEnvironment("."))
			{
				Assert.AreNotSame(environment1.Inner, environment2.Inner);
			}

		}
	}
}