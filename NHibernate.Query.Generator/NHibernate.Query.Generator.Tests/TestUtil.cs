#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
			QueryGenerator generator = new QueryGenerator(reader, new CSharpCodeProvider(), "Query", null);
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
		  cp.ReferencedAssemblies.Add(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute).Assembly.Location);
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