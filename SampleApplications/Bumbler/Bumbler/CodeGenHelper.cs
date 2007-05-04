using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Bumbler;
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
			CodeGenerator.Main(new string[] { Path.Combine(dir.FullName, "*.hbm.xml") });
			Environment.CurrentDirectory = prevDir;
		}

		public static string GenerateMapping(ITable table)
		{
			StringBuilder mapping = new StringBuilder();
			mapping.AppendLine("<?xml version=\"1.0\" ?>");
			mapping.AppendLine(
				"<hibernate-mapping namespace='Bumble' auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">");
			mapping.AppendFormat("<class name='{0}' table='{1}'>", Inflector.Singularize(table.Name) ?? table.Name, table.Name)
				.AppendLine();

			foreach (IColumn column in table.Columns)
			{
				if (column.IsPK)
				{
					mapping.AppendFormat("<id name='{0}' type='{1}'>", column.Name, column.ClrTypeName)
						.AppendLine();
					mapping.AppendLine("<generator class='native'/>");// probably not the best idea, but simplest
					mapping.AppendLine("</id>");
				}
				else if (column.IsFK)
				{
					mapping.AppendFormat("<many-to-one name='{0}' class='{1}' column='{0}' />",
										 column.Name, Inflector.Singularize(column.FkTableName) ?? column.FkTableName)
						.AppendLine();
				}
				else
				{
					mapping.AppendFormat("<property name='{0}' type='{1}'/>", column.Name, column.ClrTypeName)
						.AppendLine();
				}
			}
			mapping.AppendLine("</class>")
				.AppendLine("</hibernate-mapping>");
			return mapping.ToString();
		}

		public static Assembly GenerateAssemblyAndMappingFromSchema(ISchemaInspector inspector)
		{
			Dictionary<string, string> mappings = new Dictionary<string, string>();
			foreach (ITable table in inspector.GetTables())
			{
				if (table.HasSingleColumnPrimaryKey == false)
					continue;// want to keep is simple for now
				mappings.Add(table.Name, CodeGenHelper.GenerateMapping(table));
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