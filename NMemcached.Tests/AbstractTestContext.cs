using System.IO;
using NMemcached.Util;

namespace NMemcached.Tests
{
	public abstract class AbstractTestContext : CacheMixin
	{
		protected static string ReadAll(int initialPosition, Stream stream)
		{
			stream.Position = initialPosition;
			return new StreamReader(stream).ReadToEnd();
		}

		protected static string ReadAll(Stream stream)
		{
			return ReadAll(0, stream);
		}


		protected static MemoryStream GetStreamWithData(byte[] buffer)
		{
			var stream = new MemoryStream();
			stream.Write(buffer, 0, 4);
			var writer = new StreamWriter(stream);
			writer.WriteLine();
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}