using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Misc;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class FlushAllCommandTests : AbstractTestContext
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
		public void Specifying_too_many_parameters_will_fail()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			
			cmd.Init("foo", "bar", "baz");
			Assert.AreEqual("CLIENT_ERROR Expected 'flush_all [delay] [noreply]'\r\n", ReadAll(stream));
		}

		[Test]
		public void Specifying_non_numeric_time_parameters_fails()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init("foo", "bar");
			Assert.AreEqual("CLIENT_ERROR Exptime should be an integer\r\n", ReadAll(stream));
		}

		[Test]
		public void Specifying_invalid_noreplay_value_fails()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init("5", "bar");
			Assert.AreEqual("CLIENT_ERROR Last argument was expected to be [noreply]\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_correctly_parse_arguments_with_noreply()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init("60", "noreply");
			Assert.IsTrue(cmd.NoReply);
			Assert.AreEqual(new DateTime(2000, 1, 1, 0, 1, 0), cmd.DelayFlushUntil);
		}

		[Test]
		public void When_passing_noreply_will_not_send_any_input_to_user()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init("60", "noreply");
			cmd.Execute();
			Assert.IsEmpty(ReadAll(stream));
		}

		[Test]
		public void Can_specify_without_parameters()
		{
			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init();
			Assert.IsFalse(cmd.NoReply);
			Assert.IsNull(cmd.DelayFlushUntil);
		}

		[Test]
		public void Without_parameters_will_clear_the_cache()
		{
			for (int i = 0; i < 50; i++)
			{
				Cache[i.ToString()] = i;
			}
			Assert.AreEqual(50, Cache.Count);

			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init();
			cmd.FinishedExecuting += () => wait.Set();
			cmd.Execute();
			wait.WaitOne();

			Assert.AreEqual(0, Cache.Count);
		}

		[Test]
		public void With_timeout_will_set_expiry_in_cache()
		{
			var items = new List<CachedItem>();
			for (int i = 0; i < 50; i++)
			{
				var cachedItem = new CachedItem {ExpiresAt = SystemTime.Now()};
				items.Add(cachedItem);
				Cache[i.ToString()] = cachedItem;
			}
			Assert.AreEqual(50, Cache.Count);

			var stream = new MemoryStream();
			var cmd = new FlushAllCommand();
			cmd.SetContext(stream);
			cmd.Init("60");
			cmd.FinishedExecuting += () => wait.Set();
			cmd.Execute();
			wait.WaitOne();

			Assert.AreEqual(50, Cache.Count);
			foreach (var item in items)
			{
				Assert.AreEqual(new DateTime(2000,1,1,0,1,0), item.ExpiresAt);
			}
			Assert.AreEqual("OK\r\n", ReadAll(stream));
		}
	}
}