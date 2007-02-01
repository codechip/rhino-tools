using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace Rhino.Generators
{
	class Program
	{
		public static void Main(string[] args)
		{
			PerfromanceCountersGenerator generator = new PerfromanceCountersGenerator(new CSharpCodeProvider());
			string code = generator.Generate(File.OpenText(args[0]));
			File.WriteAllText(args[1], code);
		}
	}
}
