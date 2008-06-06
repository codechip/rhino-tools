using System;
using System.Collections;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class AddCommandTests : AbstractTestContext
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
		public void When_adding_item_will_put_it_in_cache()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			var command = new AddCommand(GetStreamWithData(buffer));
			command.Init("foo", "13", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			var cachedItem = (CachedItem)Cache.Get("foo");
			Assert.IsNotNull(cachedItem);
			CollectionAssert.AreEqual(buffer, cachedItem.Buffer);
			Assert.AreEqual(13, cachedItem.Flags);
		}


		[Test]
		public void When_adding_item_will_reply_that_it_was_stored()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new SetCommand(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_adding_item_that_already_exists_will_reply_not_stored()
		{
			MemoryStream stream = AddItem();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));

			stream = AddItem();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}

		private MemoryStream AddItem()
		{
			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new AddCommand(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();
			return stream;
		}
	}
}