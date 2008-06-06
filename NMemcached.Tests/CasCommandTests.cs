using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class CasCommandTests : AbstractTestContext
	{
		private ManualResetEvent wait;

		[SetUp]
		public void Setup()
		{
			SystemTime.Now = () => new DateTime(2000, 1, 1);
			wait = new ManualResetEvent(false);
			ClearCache();
		}

		[Test]
		public void Specifying_invalid_cas_value_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new CasCommand(stream);
			bool result = cmd.Init("b", "1", "2","2", "?x");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR cas value should be numeric\r\n", actual);
		}

		[Test]
		public void Trying_to_call_empty_key_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new CasCommand(stream);
			bool result = cmd.Init("", "1", "2", "2", "?x");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Key cannot be empty\r\n", actual);
		}

		[Test]
		public void Trying_to_call_invalid_noreply_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new CasCommand(stream);
			bool result = cmd.Init("b", "1", "2", "2", "1", "blah");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Last argument was expected to be [noreply]\r\n", actual);
		}

		[Test]
		public void Specifying_too_few_arguments_will_send_error()
		{
			var stream = new MemoryStream();
			var cmd = new CasCommand(stream);
			bool result = cmd.Init("b", "1", "2","2");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Expected to get 'cas <key> <flags> <exptime> <bytes> <cas unqiue> [noreply]'\r\n", actual);
		}

		[Test]
		public void Specifying_too_many_arguments_will_send_error()
		{
			var stream = new MemoryStream();
			var cmd = new CasCommand(stream);
			bool result = cmd.Init("b", "1", "2", "3", "noreply", "3","3");
			Assert.IsFalse(result);
			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR Expected to get 'cas <key> <flags> <exptime> <bytes> <cas unqiue> [noreply]'\r\n", actual);
		}

		[Test]
		public void Will_parse_store_arguments_include_cas()
		{
			var command = new CasCommand(new MemoryStream());
			command.Init("foo", "1", "0", "2", "523423");
			Assert.AreEqual(523423, command.Timestamp);
		}

		[Test]
		public void Will_parse_store_arguments_include_cas_with_noreply()
		{
			var command = new CasCommand(new MemoryStream());
			command.Init("foo", "1", "0", "2", "523423", "noreply");
			Assert.AreEqual(523423, command.Timestamp);
			Assert.IsTrue(command.NoReply);
		}

		[Test]
		public void Adding_item_that_does_not_exists_will_return_not_found()
		{
			var stream = new MemoryStream();
			stream.WriteByte(1);
			stream.WriteByte(2);
			stream.WriteByte(13);
			stream.WriteByte(10);
			stream.Position = 0;
			var command = new CasCommand(stream);
			command.Init("foo", "1", "0", "2", "523423");
			command.FinishedExecuting += () => wait.Set();
			
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_FOUND\r\n", ReadAll(4, stream) );
		}

		[Test]
		public void Adding_item_that_does_exists_on_cache_but_has_different_timestamp_value_will_return_exists()
		{
			Cache["foo"] = new CachedItem {Timestamp = 4};

			var stream = new MemoryStream();
			stream.WriteByte(1);
			stream.WriteByte(2);
			stream.WriteByte(13);
			stream.WriteByte(10);
			stream.Position = 0;
			var command = new CasCommand(stream);
			command.Init("foo", "1", "0", "2", "523423");
			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("EXISTS\r\n", ReadAll(4, stream));
		}

		[Test]
		public void Adding_item_that_does_exists_on_cache_and_has_matching_timestamp_value_will_return_stored()
		{
			Cache["foo"] = new CachedItem { Timestamp = 4 };

			var stream = new MemoryStream();
			stream.WriteByte(1);
			stream.WriteByte(2);
			stream.WriteByte(13);
			stream.WriteByte(10);
			stream.Position = 0;
			var command = new CasCommand(stream);
			command.Init("foo", "1", "0", "2", "4");
			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("STORED\r\n", ReadAll(4, stream));
		}

		[Test]
		public void Adding_item_that_does_exists_on_cache_and_has_matching_timestamp_value_will_replace_value()
		{
			Cache["foo"] = new CachedItem { Buffer = new byte[] { 3, 4 }, Timestamp = 4 };

			var stream = new MemoryStream();
			stream.WriteByte(1);
			stream.WriteByte(2);
			stream.WriteByte(13);
			stream.WriteByte(10);
			stream.Position = 0;
			var command = new CasCommand(stream);
			command.Init("foo", "1", "0", "2", "4");
			command.FinishedExecuting += () => wait.Set();
			command.Execute();
			wait.WaitOne();

			CachedItem c = (CachedItem)Cache["foo"];
			CollectionAssert.AreEqual(new byte[]{1,2}, c.Buffer);
			Assert.IsTrue(4L != c.Timestamp);
		}
	}
}