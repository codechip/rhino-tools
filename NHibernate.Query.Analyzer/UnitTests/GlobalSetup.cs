using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UnitTests;
using MbUnit.Framework;

[assembly: AssemblyCleanup(typeof(GlobalSetup))]

namespace Ayende.NHibernateQueryAnalyzer.UnitTests
{
	[TestFixture]
	public static class GlobalSetup
	{
		[SetUp]
		public static void Setup()
		{
			string s = File.ReadAllText(TestDataUtil.TestConfigFile)
				.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
			File.WriteAllText(TestDataUtil.TestConfigFile,s);
		}
	}
}
