using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Bumbler;
using Bumbler.Mapping;
using Castle.ActiveRecord.Framework.Internal;
using Microsoft.CSharp;
using CodeGenerator=NHibernate.Tool.hbm2net.CodeGenerator;
namespace Bumbler
{
	public static class CodeGenHelper
	{
		public static void GenerateCodeIn(string tempDir, Dictionary<string, string> mappings)
		{
			DirectoryInfo dir = new DirectoryInfo(tempDir);
			dir.Create();
			foreach (KeyValuePair<string, string> keyValuePair in mappings)
			{
				File.WriteAllText(Path.Combine(dir.FullName, keyValuePair.Key + ".hbm.xml"), keyValuePair.Value);
			}
			string prevDir = Environment.CurrentDirectory;
			Environment.CurrentDirectory = dir.FullName;
            Debug.WriteLine("Maps created in {0}",dir.FullName);
			CodeGenerator.Main(new string[] { Path.Combine(dir.FullName, "*.hbm.xml") });
			Environment.CurrentDirectory = prevDir;
		}

        private static Dictionary<string,ClassMapping> maps=new Dictionary<string, ClassMapping>();
		public static string GenerateMapping(ITable table)
		{
            ClassMapping mapping=new ClassMapping(table,maps);
            maps.Add(table.Name,mapping);
		    return mapping.ToString();
		}

		public static Assembly GenerateAssemblyAndMappingFromSchema(ISchemaInspector inspector)
		{
			Dictionary<string, string> mappings = new Dictionary<string, string>();
			foreach (ITable table in inspector.GetTables())
			{
				if (table.HasSingleColumnPrimaryKey == false)
					continue;// want to keep is simple for now
			    GenerateMapping(table);
			}
		    foreach(KeyValuePair<string, ClassMapping> map in maps)
		    {
		        map.Value.CreateOneToManyMappings(maps);
		    }
            foreach (ITable table in inspector.GetTables())
            {
                if (table.HasSingleColumnPrimaryKey == false)
                    continue;// want to keep is simple for now
                mappings.Add(table.Name,maps[table.Name].ToString());
            }

			string tempDir = Path.Combine(Path.GetTempPath(), "Bumbler_" + Guid.NewGuid());
			GenerateCodeIn(tempDir, mappings);
			return CompileAssembly(tempDir);
		}

		public static Assembly CompileAssembly(string tempDir)
		{
			Microsoft.CSharp.CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = true;
			compilerParameters.EmbeddedResources.AddRange(Directory.GetFiles(tempDir, "*.hbm.xml"));
			string codePath = Path.Combine(Path.Combine(tempDir, "generated"), "Bumble");
			CompilerResults compilerResults = codeProvider.CompileAssemblyFromFile(compilerParameters, Directory.GetFiles(codePath, "*.cs"));
			if (compilerResults.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder("Failed to compile run time generated code! ")
					.AppendLine();
				foreach (CompilerError compilerError in compilerResults.Errors)
				{
					sb.AppendLine(compilerError.ToString());
				}
				throw new InvalidOperationException(sb.ToString());
			}
			return compilerResults.CompiledAssembly;
		}
	}
}
