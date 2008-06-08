using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Storage;

namespace NMemcached.Tests
{
	[TestFixture]
	public class ReplaceCommandTests : AbstractTestContext
	{

		private ManualResetEvent wait;

		[SetUp]
		public void Setup()
		{
			SystemTime.Now = () => DateTime.Now;
			wait = new ManualResetEvent(false);
			ClearCache();
		}

		[Test]
		public void When_replacing_item_not_on_cache_will_reply_that_it_was_not_stored()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new ReplaceCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_replacing_item_on_cache_will_reply_that_it_was_not_stored()
		{
			SetItem();

			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new ReplaceCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}

		private void SetItem()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new SetCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();
		}
	}
}