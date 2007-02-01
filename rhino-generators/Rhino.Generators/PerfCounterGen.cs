using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CustomToolGenerator;
using Microsoft.CSharp;

namespace Rhino.Generators
{
	[ComVisible(true)]
	[Guid("EA132F6E-92D5-4549-B39F-F34BBC8B57DF")]
	public class PerfCounterGen : BaseCodeGeneratorWithSite
	{
		protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
		{
			PerfromanceCountersGenerator generator = new PerfromanceCountersGenerator(new CSharpCodeProvider());
			MemoryStream ms = new MemoryStream();
			using(StreamWriter sw = new StreamWriter(ms))
			{
				sw.Write(inputFileContent);
			}
			return Encoding.UTF8.GetBytes(generator.Generate(ms));
		}
	}
}
