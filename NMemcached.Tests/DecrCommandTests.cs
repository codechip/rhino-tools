using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Caching;
using MbUnit.Framework;
using NMemcached.Commands.Modifications;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class DecrCommandTests : AbstractTestContext
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
		public void When_Decrementing_value_on_cache_below_zero_will_set_to_zero()
		{
			Cache["foo"] = new CachedItem { Buffer = Encoding.ASCII.GetBytes("12") };
			var stream = new MemoryStream();
			var command = new DecrCommand(stream);
			command.Init("foo", "15");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("0\r\n", ReadAll(stream));
		}

		[Test]
		public void When_Decrementing_value_on_cache_which_is_in_valid_format_use_this_as_base()
		{
			Cache["foo"] = new CachedItem { Buffer = Encoding.ASCII.GetBytes("12") };
			var stream = new MemoryStream();
			var command = new DecrCommand(stream);
			command.Init("foo", "5");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("7\r\n", ReadAll(stream));

			wait.Reset();
			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			Assert.AreEqual("2\r\n", ReadAll(3, stream));
		}
	}
}