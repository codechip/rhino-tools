using System;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Utilties
{
	[TestFixture]
	public class ValidationTests
	{
		[Test]
		public void NoValue()
		{
			Validation.NotNull();
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SingleNullArg()
		{
			Validation.NotNull(null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SecondNullArg()
		{
			Validation.NotNull("test", null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void FirstNullSecondFine()
		{
			Validation.NotNull(null, "test");
		}

		[Test]
		public void ArgsNotNull()
		{
			Validation.NotNull("test", "second test");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ArgsNotNullWithParameters()
		{
			MethodWithArg(null);
		}

		private void MethodWithArg(object arg)
		{
			Validation.NotNull(arg);
		}
	}
}