using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class AppendCommandTests : AbstractTestContext
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
		public void When_appending_item_not_on_cache_will_reply_that_it_was_not_stored()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new AppendCommand(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}


		[Test]
		public void When_appending_item_on_cache_will_reply_with_stored()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			ICommand command = new SetCommand(GetStreamWithData(buffer));
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			wait.Reset();

			buffer = new byte[] { 5, 6, 7, 8 };
			MemoryStream stream = GetStreamWithData(buffer);
			command = new AppendCommand(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_appending_item_on_cache_will_append_to_data_already_on_cache()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			ICommand command = new SetCommand(GetStreamWithData(buffer));
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			wait.Reset();

			buffer = new byte[] { 5, 6, 7, 8 };
			MemoryStream stream = GetStreamWithData(buffer);
			command = new AppendCommand(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			CachedItem item = (CachedItem)Cache.Get("foo");

			CollectionAssert.AreEqual(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, item.Buffer);
		}

		[Test]
		public void When_appending_item_on_cache_will_not_modify_flags()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			ICommand command = new SetCommand(GetStreamWithData(buffer));
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			wait.Reset();

			buffer = new byte[] { 5, 6, 7, 8 };
			MemoryStream stream = GetStreamWithData(buffer);
			command = new AppendCommand(stream);
			command.Init("foo", "15", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			CachedItem item = (CachedItem)Cache.Get("foo");

			Assert.AreEqual(1, item.Flags);
		}
	}
}