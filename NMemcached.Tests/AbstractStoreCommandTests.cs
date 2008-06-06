using System;
using System.IO;
using MbUnit.Framework;
using NMemcached.Commands.Storage;

namespace NMemcached.Tests
{
	[TestFixture]
	public class AbstractStoreCommandTests : AbstractTestContext
	{
		[SetUp]
		public void Setup()
		{
			SystemTime.Now = () => new DateTime(2000, 1, 1);
		}

		[Test]
		public void When_initializing_will_parse_command_line_without_noreply()
		{
			var cmd = new TestCommand(new MemoryStream());
			cmd.Init("my_key", "1234", "60", "50");
			Assert.AreEqual("my_key", cmd.Key);
			Assert.AreEqual(1234, cmd.Flags);
			Assert.AreEqual(new DateTime(2000, 1, 1, 0, 1, 0), cmd.ExpiresAt);
			Assert.AreEqual(50, cmd.BytesCount);
			Assert.IsFalse(cmd.NoReply);
		}

		[Test]
		public void Passing_expiration_zero_will_never_expire()
		{
			var cmd = new TestCommand(new MemoryStream());
			cmd.Init("my_key", "1234", "0", "50");
			Assert.AreEqual(new DateTime(2100, 1, 1, 0, 0, 0), cmd.ExpiresAt);
		}


		[Test]
		public void Will_error_if_key_is_empty()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("", "1", "2", "3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Key cannot be empty\r\n", ReadAll(stream));
		}


		[Test]
		public void Specifying_too_many_arguments_will_send_error()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("b", "1", "2", "3","noreply","3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Expected to get 'test <key> <flags> <exptime> <bytes> [noreply]'\r\n", ReadAll(stream));
		}


		[Test]
		public void Will_error_if_bytes_is_not_numeric()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("123", "1", "2", "x3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Bytes should be a numeric value\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_bytes_is_non_negative()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("123", "1", "2", "-3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Bytes cannot be negative\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_flags_is_not_integer()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("x", "v", "2", "3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Flags should be an 32 bits integer\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_expiry_is_not_integer()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("x", "2", "x", "3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Exptime should be an integer\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_key_is_null()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init(null, "1", "2", "3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Key cannot be empty\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_key_is_longer_than_250_charaters()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init(new string('a', 251), "1", "2", "3");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Key cannot be larger than 250 characters\r\n", ReadAll(stream));
		}

		[Test]
		public void Will_error_if_get_less_than_4_parameters()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			bool result = cmd.Init("my_key");
			Assert.IsFalse(result);
			Assert.AreEqual("CLIENT_ERROR Expected to get 'test <key> <flags> <exptime> <bytes> [noreply]'\r\n", ReadAll(stream));
		}

		[Test]
		public void When_initializing_will_parse_command_line_with_noreply()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			cmd.Init("my_key", "1234", "60", "50", "noreply");
			Assert.AreEqual("my_key", cmd.Key);
			Assert.AreEqual(1234, cmd.Flags);
			Assert.AreEqual(new DateTime(2000, 1, 1, 0, 1, 0), cmd.ExpiresAt);
			Assert.IsTrue(cmd.NoReply);
		}

		[Test]
		public void When_expiry_is_larger_than_30_days_will_use_unix_epoch()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			string expiry = TimeSpan.FromDays(31).TotalSeconds.ToString();
			cmd.Init("my_key", "1234", expiry, "50", "noreply");
			Assert.AreEqual(new DateTime(1970, 1, 1).AddDays(31), cmd.ExpiresAt);
		}

		[Test]
		public void When_calling_send_to_client_will_write_to_stream()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			cmd.Init("my_key", "1234", "1", "50");
			cmd.SendToClient();
			Assert.AreEqual("foo\r\n", ReadAll(stream));
		}

		[Test]
		public void When_calling_send_to_client_when_noreply_specified_will_NOT_write_to_stream()
		{
			var stream = new MemoryStream();
			var cmd = new TestCommand(stream);
			cmd.Init("my_key", "1234", "1", "50", "noreply");
			cmd.SendToClient();
			Assert.AreEqual("", ReadAll(stream));
		}
		
		#region Nested type: TestCommand

		public class TestCommand : AbstractStoreCommand
		{
			public TestCommand(Stream stream) : base(stream)
			{
			}

			protected override void ExecuteCommand()
			{
				throw new NotImplementedException();
			}

			public void SendToClient()
			{
				SendToClient("foo");
			}
		}

		#endregion
	}
}