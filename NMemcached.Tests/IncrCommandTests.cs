using System;
using System.IO;
using System.Text;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Modifications;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class IncrCommandTests : AbstractTestContext
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
		public void Specifying_too_few_arguments_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new IncrCommand(stream);
			cmd.Init("a");
			Assert.AreEqual("CLIENT_ERROR Expected to get 'incr <key> <values> [noreply]'\r\n", ReadAll(stream));
		}

		[Test]
		public void Specifying_too_many_arguments_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new IncrCommand(stream);
			cmd.Init("a","1","noreply", "e");
			Assert.AreEqual("CLIENT_ERROR Expected to get 'incr <key> <values> [noreply]'\r\n", ReadAll(stream));
		}

		[Test]
		public void Specifying_non_numeric_value_will_result_in_error()
		{
			var stream = new MemoryStream();
			var cmd = new IncrCommand(stream);
			cmd.Init("a", "x1", "noreply");
			Assert.AreEqual("CLIENT_ERROR Value should be a numeric value\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_parse_values_from_command_args_with_no_reply()
		{
			var stream = new MemoryStream();
			var cmd = new IncrCommand(stream);
			cmd.Init("a", "1", "noreply");
			Assert.AreEqual("a", cmd.Key);
			Assert.AreEqual(1UL, cmd.Value);
			Assert.IsTrue(cmd.NoReply);
		}

		[Test]
		public void Will_parse_values_from_command_args_without_noreply()
		{
			var stream = new MemoryStream();
			var cmd = new IncrCommand(stream);
			cmd.Init("a", "1");
			Assert.AreEqual("a", cmd.Key);
			Assert.AreEqual(1UL, cmd.Value);
			Assert.IsFalse(cmd.NoReply);
		}

		[Test]
		public void When_incrementing_value_no_on_cache_will_return_not_found()
		{
			var stream = new MemoryStream();
			var command = new IncrCommand(stream);
			command.Init("foo", "1");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("NOT_FOUND\r\n", ReadAll(stream));
		}

		[Test]
		public void When_incrementing_value_on_cache_which_is_in_invalid_format_assumes_it_is_zero()
		{
			Cache["foo"] = new CachedItem {Buffer = new byte[] {1}};
			var stream = new MemoryStream();
			var command = new IncrCommand(stream);
			command.Init("foo", "5");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("5\r\n", ReadAll(stream));
		}

		[Test]
		public void When_incrementing_value_on_cache_which_is_in_valid_format_use_this_as_base()
		{
			Cache["foo"] = new CachedItem { Buffer = Encoding.ASCII.GetBytes("12") };
			var stream = new MemoryStream();
			var command = new IncrCommand(stream);
			command.Init("foo", "5");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("17\r\n", ReadAll(stream));

			wait.Reset();
			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("22\r\n", ReadAll(4, stream));
		}
	}
}