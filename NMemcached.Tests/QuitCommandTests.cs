using System.IO;
using MbUnit.Framework;
using NMemcached.Commands.Misc;

namespace NMemcached.Tests
{
	[TestFixture]
	public class QuitCommandTests : AbstractTestContext
	{
		[Test]
		public void When_passing_arguments_to_quit_will_send_error()
		{
			var stream = new MemoryStream();
			var quit = new QuitCommand(stream, null);
			quit.Init("foo");
			Assert.AreEqual("CLIENT_ERROR Quit accepts no paramters\r\n", ReadAll(stream));
		}

		[Test]
		public void When_passing_zero_arguments_to_quit_will_succeed()
		{
			var stream = new MemoryStream();
			var quit = new QuitCommand(stream, null);
			Assert.IsTrue(quit.Init());
		}

		[Test]
		public void When_calling_execute_will_call_action()
		{
			var stream = new MemoryStream();
			bool wasCalled = false;
			var quit = new QuitCommand(stream, () => wasCalled = true);
			quit.Execute();
			Assert.IsTrue(wasCalled);
		}
	}
}