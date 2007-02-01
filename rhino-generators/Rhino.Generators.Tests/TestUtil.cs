using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Rhino.Generators.Tests;

namespace Rhino.Generators.Tests
{
	class TestUtil
	{
		public static Assembly GetAssemblyFromCode(string code)
		{
			CompilerResults results = CompileCode(code);
			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new	StringBuilder();
				foreach (CompilerError error in results.Errors)
				{
					sb.Append(error.ErrorText).AppendLine();
				}
				throw new InvalidOperationException("Compilation errors\r\n"+sb.ToString());
			}
			return results.CompiledAssembly;
		}

		public static CompilerResults CompileCode(string code)
		{
			CodeDomProvider provider = new CSharpCodeProvider();
			CompilerParameters cp = new CompilerParameters();
			cp.GenerateInMemory = true;
			cp.OutputAssembly = "Generated.Context";
			cp.ReferencedAssemblies.Add(typeof(TestUtil).Assembly.Location);
			cp.ReferencedAssemblies.Add(typeof (PerformanceCounter).Assembly.Location);
			return provider.CompileAssemblyFromSource(cp, code);
		}
	}
}