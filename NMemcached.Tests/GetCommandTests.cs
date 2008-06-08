using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Commands.Retrieval;
using NMemcached.Model;

namespace NMemcached.Tests
{
	[TestFixture]
	public class GetCommandTests : AbstractTestContext
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
		public void Calling_without_any_keys_will_result_in_error()
		{
			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);
			command.Init();

			string actual = ReadAll(stream);
			Assert.AreEqual("CLIENT_ERROR At least one key should be specified\r\n", actual);
		}

		[Test]
		public void When_getting_item_that_is_not_in_cache_will_return_empty_result_set()
		{
			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);

			command.Init("foo");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			string actual = ReadAll(stream);
			Assert.AreEqual("END\r\n", actual);
		}

		[Test]
		public void When_getting_item_that_is_in_cache_will_return_item()
		{
			Cache["foo"] = new CachedItem
			               	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo", ExpiresAt = SystemTime.Now().AddDays(1)};

			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);

			command.Init("foo");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			stream.Position = 0;
			string line = new StreamReader(stream).ReadLine();
			Assert.AreEqual("VALUE foo 2 3", line);
			stream.Position = line.Length + 2; // reset buffering of stream reader
			var buffer = new byte[5];
			stream.Read(buffer, 0, 5);
			CollectionAssert.AreEqual(new byte[] {1, 2, 3, 13, 10}, buffer);
			Assert.AreEqual("END", new StreamReader(stream).ReadLine());
		}

		[Test]
		public void When_getting_item_that_has_been_expired_will_return_empty_result()
		{
			Cache["foo"] = new CachedItem
			               	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo", ExpiresAt = SystemTime.Now().AddDays(-1)};

			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);

			command.Init("foo");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			stream.Position = 0;
			Assert.AreEqual("END", new StreamReader(stream).ReadLine());
		}

		[Test]
		public void When_getting_several_items_that_are_in_cache_will_return_items()
		{
			Cache["foo0"] = new CachedItem
			                	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo0", ExpiresAt = SystemTime.Now().AddDays(1)};
			Cache["foo1"] = new CachedItem
			                	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo1", ExpiresAt = SystemTime.Now().AddDays(1)};
			Cache["foo2"] = new CachedItem
			                	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo2", ExpiresAt = SystemTime.Now().AddDays(1)};

			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);

			command.Init("foo0", "foo1", "foo2");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			stream.Position = 0;
			int currentPos = 0;
			for (int i = 0; i < 3; i++)
			{
				string line = new StreamReader(stream).ReadLine();
				Assert.AreEqual("VALUE foo" + i + " 2 3", line);
				stream.Position = currentPos + line.Length + 2; // reset buffering of stream reader
				var buffer = new byte[5];
				stream.Read(buffer, 0, 5);
				CollectionAssert.AreEqual(new byte[] {1, 2, 3, 13, 10}, buffer);
				currentPos = (int) stream.Position;
			}
			Assert.AreEqual("END", new StreamReader(stream).ReadLine());
		}

		[Test]
		public void When_getting_several_items_where_some_in_cache_and_some_not_will_return_items_that_are_in_cache()
		{
			Cache["foo0"] = new CachedItem
			                	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo0", ExpiresAt = SystemTime.Now().AddDays(1)};
			Cache["foo2"] = new CachedItem
			                	{Buffer = new byte[] {1, 2, 3}, Flags = 2, Key = "foo2", ExpiresAt = SystemTime.Now().AddDays(1)};

			var stream = new MemoryStream();
			var command = new GetCommand();
			command.SetContext(stream);

			command.Init("foo0", "foo1", "foo2");

			command.FinishedExecuting += () => { wait.Set(); };
			command.Execute();
			wait.WaitOne();

			stream.Position = 0;
			int currentPos = 0;
			for (int i = 0; i < 3; i++)
			{
				if (i == 1)
					continue;
				string line = new StreamReader(stream).ReadLine();
				Assert.AreEqual("VALUE foo" + i + " 2 3", line);
				stream.Position = currentPos + line.Length + 2; // reset buffering of stream reader
				var buffer = new byte[5];
				stream.Read(buffer, 0, 5);
				CollectionAssert.AreEqual(new byte[] {1, 2, 3, 13, 10}, buffer);
				currentPos = (int) stream.Position;
			}
			Assert.AreEqual("END", new StreamReader(stream).ReadLine());
		}
	}
}