using System;
using System.IO;
using System.Runtime.InteropServices;
using CustomToolGenerator;
using Microsoft.CSharp;

namespace NHibernate.Query.Generator
{
	[ComVisible(true)]
	[Guid("DE8A7135-96F6-47ba-9DC9-D7D837B4CDE3")]
	public class NHibernateQueryGenerator : BaseCodeGeneratorWithSite
	{
		/// <summary>
		/// the method that does the actual work of generating code given the input
		/// file.
		/// </summary>
		/// <param name="inputFileName">input file name</param>
		/// <param name="inputFileContent">file contents as a string</param>
		/// <returns>the generated code file as a byte-array</returns>
		protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
		{
			//System.Diagnostics.Debugger.Break();
			using (MemoryStream ms = new MemoryStream())
			{
				using (StringReader reader = new StringReader(inputFileContent))
				{
					QueryGenerator qg = new QueryGenerator(reader, CodeProvider, "Query");
					using (TextWriter writer = new StreamWriter(ms))
					{
						qg.Generate(writer);
						writer.Flush();
						return ms.ToArray();
					}
				}
			}
		}
	}
}
