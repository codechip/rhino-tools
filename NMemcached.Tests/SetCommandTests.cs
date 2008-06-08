using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class SetCommandTests : AbstractTestContext
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
		public void When_setting_item_will_put_it_in_cache()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			var command = new SetCommand();
			command.SetContext(GetStreamWithData(buffer));

			command.Init("foo", "1","6000","4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			var cachedItem = (CachedItem)Cache.Get("foo");
			Assert.IsNotNull(cachedItem);
			CollectionAssert.AreEqual(buffer, cachedItem.Buffer);
			Assert.AreEqual(1, cachedItem.Flags);
		}

		[Test]
		public void When_setting_item_will_reply_that_it_was_stored()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new SetCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}


		[Test]
		public void When_item_does_not_end_in_CRLF_will_send_error()
		{
			var buffer = new byte[] { 1, 2, 3, 4 };
			var stream = new MemoryStream();
			stream.Write(buffer, 0, 4);
			stream.Position = 0;
			var command = new SetCommand();
			command.SetContext(stream);
			
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("CLIENT_ERROR Data section should end with a line break (\\r\\n)\r\n", ReadAll(4, stream));
		}

		[Test]
		public void When_item_end_in_just_LF_will_send_error()
		{
			var buffer = new byte[] { 1, 2, 3, 4, 10 };
			var stream = new MemoryStream();
			stream.Write(buffer, 0, 5);
			stream.Position = 0;
			var command = new SetCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("CLIENT_ERROR Data section should end with a line break (\\r\\n)\r\n", ReadAll(5, stream));
		}

		[Test]
		public void When_item_end_in_just_CR_will_send_error()
		{
			var buffer = new byte[] { 1, 2, 3, 4, 13 };
			var stream = new MemoryStream();
			stream.Write(buffer, 0, 5);
			stream.Position = 0;
			var command = new SetCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("CLIENT_ERROR Data section should end with a line break (\\r\\n)\r\n", ReadAll(5, stream));
		}


		[Test]
		public void When_setting_item_that_already_exists_will_reply_with_stored()
		{
			MemoryStream stream = SetItem();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));

			stream = SetItem();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}

		private MemoryStream SetItem()
		{
			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			MemoryStream stream = GetStreamWithData(buffer);
			var command = new SetCommand();
			command.SetContext(stream);
			command.Init("foo", "1", "6000", "4");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();
			return stream;
		}
	}
}