using System.IO;
using System.Net.Sockets;
using MbUnit.Framework;
using NMemcached.Commands.Misc;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class QuitAndVersionTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Can_get_version_from_server()
		{
			using (var tcpClient = new TcpClient("127.0.0.1", 33433))
			using (var stream = tcpClient.GetStream())
			using (var sr = new StreamReader(stream))
			using (var sw = new StreamWriter(stream))
			{
				sw.WriteLine("version");
				sw.Flush();

				string version = sr.ReadLine();
				Assert.AreEqual("VERSION " + typeof(VersionCommand).Assembly.GetName().Version, version);
			}
		}

		[Test]
		public void Calling_quit_will_close_the_connection()
		{
			using (var tcpClient = new TcpClient("127.0.0.1", 33433))
			using (var stream = tcpClient.GetStream())
			using (var sr = new StreamReader(stream))
			using (var sw = new StreamWriter(stream))
			{
				sw.WriteLine("quit");
				sw.Flush();

				Assert.AreEqual(-1, stream.ReadByte());
			}
		}
	}
}