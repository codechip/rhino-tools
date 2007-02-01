using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Rhino.Generators.Tests
{
	[TestFixture]
	public class PerformanceGeneratorFixture
	{
		[Test]
		public void CanGenerateDerivedClass()
		{
			PerfromanceCountersGenerator generator = new PerfromanceCountersGenerator(new CSharpCodeProvider());
			string code = generator.Generate(File.OpenText(@"D:\OSS\rhino-tools\rhino-generators\Rhino.Generators.Tests\Performance.cs"));
			Assembly assembly = TestUtil.GetAssemblyFromCode(code);
			Assert.IsNotNull(assembly);
		}
	}
}
    