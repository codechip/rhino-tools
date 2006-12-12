using System.IO;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts
{
	public sealed class FileAssert
	{
		private FileAssert()
		{
		}

		public static void Exists(string file)
		{
			Exists(file, string.Empty);
		}

		public static void Exists(string file, string message, params object[] args)
		{
			Assert.DoAssert(new FileExists(file, message, args));
		}

	}

	internal class FileExists : AssertBase
	{
		private readonly string file;

		public FileExists(string file, string message, object[] args) : base(message, args)
		{
			this.file = file;

		}

		public override void Assert()
		{
			if (File.Exists(file) == false)
				Fail();
		}
	}
}