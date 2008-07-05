using System.IO;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class BerkeleyDbTransactionTests
	{
		[SetUp]
		public void Setup()
		{
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");
			
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				environment.Delete("my-queue");
			}
		}

		[Test]
		public void Can_create_queue_without_transaction()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				environment.CreateQueue("my-queue", 100);
			}
		}

		[Test]
		public void Can_create_queue_with_transaction()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			{
				environment.CreateQueue("my-queue", 100);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
				Assert.IsTrue(environment.Exists("my-queue"));
		}

		[Test]
		public void Can_create_queue_with_transaction_and_rollback()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			{
				environment.CreateQueue("my-queue", 100);
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
				Assert.IsFalse(environment.Exists("my-queue"));
		}
	}
}