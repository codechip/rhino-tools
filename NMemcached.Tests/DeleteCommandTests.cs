using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class DeleteCommandTests : AbstractTestContext
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
		public void Will_parse_key_only_args()
		{
			var cmd = new DeleteCommand();
			cmd.SetContext(new MemoryStream());
			bool result = cmd.Init("foo");
			Assert.IsTrue(result);
			Assert.AreEqual("foo", cmd.Key);
			Assert.IsFalse(cmd.NoReply);
			Assert.IsNull(cmd.BlockedFromUpdatingUntil);
		}

		[Test]
		public void Will_parse_key_and_time_args()
		{
			SystemTime.Now = () => new DateTime(2000,1,1);
			var cmd = new DeleteCommand();
			cmd.SetContext(new MemoryStream());
			bool result = cmd.Init("foo", "60");
			Assert.IsTrue(result);
			Assert.AreEqual("foo", cmd.Key);
			Assert.IsFalse(cmd.NoReply);
			Assert.AreEqual(new DateTime(2000, 1, 1, 0, 1, 0), cmd.BlockedFromUpdatingUntil);
		}

		[Test]
		public void When_time_is_zero_will_parse_as_if_null()
		{
			var cmd = new DeleteCommand();
			cmd.SetContext(new MemoryStream());
			bool result = cmd.Init("foo", "0");
			Assert.IsTrue(result);
			Assert.AreEqual("foo", cmd.Key);
			Assert.IsFalse(cmd.NoReply);
			Assert.IsNull(cmd.BlockedFromUpdatingUntil);
		}

		[Test]
		public void When_noreply_is_specified_NoReply_equal_to_true()
		{
			var cmd = new DeleteCommand();
			cmd.SetContext(new MemoryStream());
			bool result = cmd.Init("foo", "0", "noreply");
			Assert.IsTrue(result);
			Assert.AreEqual("foo", cmd.Key);
			Assert.IsTrue(cmd.NoReply);
			Assert.IsNull(cmd.BlockedFromUpdatingUntil);
		}

		[Test]
		public void Specifying_invalid_time_value_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new DeleteCommand();
			cmd.SetContext(stream);
			bool result = cmd.Init("b", "?x");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Exptime should be an integer\r\n", actual);
		}


		[Test]
		public void Specifying_too_few_arguments_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new DeleteCommand();
			cmd.SetContext(stream);
			bool result = cmd.Init();
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Expected 'delete <key> [<time>] [noreply]'\r\n", actual);
		}

		[Test]
		public void Specifying_too_many_arguments_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new DeleteCommand();
			cmd.SetContext(stream);
			bool result = cmd.Init("a", "2", "noreply", "bar");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Expected 'delete <key> [<time>] [noreply]'\r\n", actual);
		}

		[Test]
		public void Specifying_invalid_noreply_value_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new DeleteCommand();
			cmd.SetContext(stream);
			bool result = cmd.Init("a", "2", "bar");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Last argument was expected to be [noreply]\r\n", actual);
		}

		[Test]
		public void When_deleting_item_in_cache_will_remove_from_cache()
		{
			Cache["foo"] = new CachedItem();

			var command = new DeleteCommand();
			command.SetContext(new MemoryStream());
			command.Init("foo");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			var cachedItem = (CachedItem)Cache.Get("foo");
			Assert.IsNull(cachedItem);
		}

		[Test]
		public void When_deleting_item_in_cache_will_return_deleted()
		{
			Cache["foo"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("DELETED\r\n", ReadAll(stream));
		}

		[Test]
		public void When_deleting_item_not_in_cache_will_return_not_found()
		{
			Cache["foo"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_FOUND\r\n", ReadAll(stream));
		}

		[Test]
		public void When_deleting_item_not_in_cache_will_return_nothing_with_no_reply()
		{
			Cache["foo"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2", "0", "noreply");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("", ReadAll(stream));
		}

		[Test]
		public void When_deleting_item_in_cache_with_time_will_block_add_operations()
		{
			Cache["foo2"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2", "500");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("DELETED\r\n", ReadAll(stream));

			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			stream = GetStreamWithData(buffer);
			var addCommand = new AddCommand();
			addCommand.SetContext(stream);
			addCommand.Init("foo2", "1", "6000", "4");

			addCommand.FinishedExecuting += () => wait.Set();
			addCommand.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_deleting_item_in_cache_with_time_when_item_do_not_exists_should_not_block_add_operations()
		{
			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2", "500");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_FOUND\r\n", ReadAll(stream));

			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			stream = GetStreamWithData(buffer);
			var addCommand = new AddCommand();
			addCommand.SetContext(stream);
			addCommand.Init("foo2", "1", "6000", "4");

			addCommand.FinishedExecuting += () => wait.Set();
			addCommand.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_deleting_item_in_cache_with_time_will_block_replace_operations()
		{
			Cache["foo2"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2", "500");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("DELETED\r\n", ReadAll(stream));

			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			stream = GetStreamWithData(buffer);
			var replaceCommand = new ReplaceCommand();
			replaceCommand.SetContext(stream);
			replaceCommand.Init("foo2", "1", "6000", "4");

			replaceCommand.FinishedExecuting += () => wait.Set();
			replaceCommand.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}

		[Test]
		public void When_deleting_item_in_cache_with_time_will_block_cas_operations()
		{
			Cache["foo2"] = new CachedItem();

			var stream = new MemoryStream();
			var command = new DeleteCommand();
			command.SetContext(stream);
			command.Init("foo2", "500");

			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("DELETED\r\n", ReadAll(stream));

			wait.Reset();

			var buffer = new byte[] { 1, 2, 3, 4 };
			stream = GetStreamWithData(buffer);
			var casCommand = new CasCommand();
			casCommand.SetContext(stream);
			casCommand.Init("foo2", "1", "6000", "4", "2");

			casCommand.FinishedExecuting += () => wait.Set();
			casCommand.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_STORED\r\n", ReadAll(6, stream));
		}
	}
}