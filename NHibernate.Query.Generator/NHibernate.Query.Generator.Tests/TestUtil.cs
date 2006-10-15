using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Query;

namespace NHibernate.Query.Generator.Tests
{
	class TestUtil
	{
		public static string GenerateCode(Stream sampleStream)
		{
			StringBuilder sb = new StringBuilder();

			TextReader reader = new StreamReader(sampleStream);
			TextWriter writer = new StringWriter(sb);
			QueryGenerator generator = new QueryGenerator(reader, new CSharpCodeProvider());
			generator.Generate(writer);

			return sb.ToString();
		}

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
			cp.ReferencedAssemblies.Add(typeof(ISession).Assembly.Location);
			cp.ReferencedAssemblies.Add(typeof(QueryBuilder<>).Assembly.Location);

			//need this for the model assemblies
			cp.ReferencedAssemblies.Add(typeof(TestUtil).Assembly.Location);

			return provider.CompileAssemblyFromSource(cp, code);
		}

		public static Assembly GetAssemblyFromResource(string resource)
		{
			string generateCode = TestUtil.GenerateCode(typeof (TestUtil).Assembly.GetManifestResourceStream(resource));
			return TestUtil.GetAssemblyFromCode(generateCode);
		}
	}
}