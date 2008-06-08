using System;
using System.IO;
using MbUnit.Framework;
using NMemcached.Commands.Misc;

namespace NMemcached.Tests
{
	[TestFixture]
	public class VersionCommandTests : AbstractTestContext
	{
		[Test]
		public void Cannot_pass_arguments_to_version()
		{
			var stream = new MemoryStream();
			var cmd = new VersionCommand();
			cmd.SetContext(stream);
			cmd.Init("a");
			Assert.AreEqual("CLIENT_ERROR Version accepts no paramters\r\n", ReadAll(stream));
		}

		[Test]
		public void Calling_init_with_no_params_succeed()
		{
			var stream = new MemoryStream();
			var cmd = new VersionCommand();
			cmd.SetContext(stream);
			
			Assert.IsTrue(cmd.Init());
		}

		[Test]
		public void Will_return_assembly_version_as_memcached_version()
		{
			var stream = new MemoryStream();
			var cmd = new VersionCommand();
			cmd.SetContext(stream);
			
			cmd.Execute();
			Assert.AreEqual("VERSION " + typeof(VersionCommand).Assembly.GetName().Version 
				+ "\r\n", ReadAll(stream));
		}
	}
}