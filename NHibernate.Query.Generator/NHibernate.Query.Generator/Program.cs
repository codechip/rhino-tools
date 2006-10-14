using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace NHibernate.Query.Generator
{
	class Program
	{
		private static string extention;
		private static string outputDir;
		private static CodeDomProvider provider = null;

		static void Main(string[] args)
		{
			string inputFilePattern = GetCommandLineArguments(args);
			try
			{
				SetupCodeProvider();
				string directoryName = Path.GetDirectoryName(inputFilePattern);
				string fileName = Path.GetFileName(inputFilePattern);
				foreach (string file in Directory.GetFiles(directoryName, fileName))
				{
					OutputFile(file);
				}
				OutputQueryBuilder();
			}
			catch (ReflectionTypeLoadException e)
			{
				Console.WriteLine("A type load error occured!\r\nThis usually happens if NHibernate Query Generator is unable to load all the required assemblies.");
				Dictionary<string, bool> reported = new Dictionary<string, bool>();
				foreach (Exception loaderException in e.LoaderExceptions)
				{
					if (reported.ContainsKey(loaderException.Message) == false)
					{
						Console.WriteLine(loaderException.Message);
						reported.Add(loaderException.Message, true);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("An error occured:");
				Console.Write(e);
			}
		}

		private static void OutputFile(string file)
		{
			string extension = Path.GetExtension(file);
			string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + extension);
			// hbm file
			if (extension.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase))
			{
				GenerateSingleFile(File.OpenText(file), outputFile);
			}
			else if (extention.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) ||
			         extension.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase))// Active Record...
			{
				GenerateFromActiveRecordAssembly(file);
			}
		}

		private static void OutputQueryBuilder()
		{
//write query builders so user can just include the whole directory.
			Stream namedExp = typeof(Program).Assembly.GetManifestResourceStream("NHibernate.Query.Generator.QueryBuilders.QueryBuilder."+extention);
			File.WriteAllText(Path.Combine(outputDir, "QueryBuilder." + extention), new StreamReader(namedExp).ReadToEnd());
			Console.WriteLine("Successfuly created file: {0}\\NamedExpression.{1}", outputDir, extention);
		}

		private static void GenerateFromActiveRecordAssembly(string file)
		{
			string fullPath = Path.GetFullPath(file);
			RegisterAssemblyResolver(fullPath);

			Assembly asm = Assembly.LoadFile(fullPath);
			ActiveRecordModelBuilder activeRecordModelBuilder = new ActiveRecordModelBuilder();
			List<ActiveRecordModel> models = new List<ActiveRecordModel>();
			foreach (System.Type type in asm.GetTypes())
			{
				if (type.IsDefined(typeof(ActiveRecordAttribute), true) == false)
					continue;
				ActiveRecordModel model = activeRecordModelBuilder.Create(type);
				if (model == null)
					continue;
				models.Add(model);
				GraphConnectorVisitor graphConnectorVisitor = new GraphConnectorVisitor(activeRecordModelBuilder.Models);
				graphConnectorVisitor.VisitModel(model);
			}

			foreach (ActiveRecordModel model in models)
			{
				XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
				SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(activeRecordModelBuilder.Models);
				semanticVisitor.VisitNode(model);
				xmlVisitor.CreateXml(model);
				string genFile = Path.Combine(outputDir, model.Type.Name + "." + extention);
				GenerateSingleFile(new StringReader(xmlVisitor.Xml), genFile);
			}
		}

		private static void RegisterAssemblyResolver(string fullPath)
		{
			string dir = Path.GetDirectoryName(fullPath);
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
			{
				string asmFileName = args.Name.Split(',')[0];
				string exeFileName = Path.Combine(dir, asmFileName + ".exe");
				if (File.Exists(exeFileName))
					return Assembly.LoadFile(exeFileName);
				string dllFileName = Path.Combine(dir, asmFileName + ".dll");
				if (File.Exists(dllFileName))
					return Assembly.LoadFile(dllFileName);
				return null;
			};
		}

		private static void GenerateSingleFile(TextReader input, string destinationFile)
		{
			QueryGenerator generator = new QueryGenerator(input, provider);
			using (StreamWriter outputStream = File.CreateText(destinationFile))
			{
				generator.Generate(outputStream);
			}
			Console.WriteLine("Successfuly created file {0}", destinationFile);
		}

		private static string GetCommandLineArguments(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("      NHibernate.Query.Generator <cs or vb> <*.hbm.xml> <output-dir>");
				Console.WriteLine("      NHibernate.Query.Generator <cs or vb> asssembly.dll <output-dir>");
				Environment.Exit(1);
			}
			extention = args[0];
			outputDir = args[2];
			return args[1];
		}

		private static void SetupCodeProvider()
		{
			switch (extention.ToLower())
			{
				case "vb":
					provider = new VBCodeProvider();
					break;
				case "cs":
					provider = new CSharpCodeProvider();
					break;
				default:
					Console.WriteLine("Unknown language element, expected 'cs' or 'vb', but got {0}", extention);
					Environment.Exit(1);
					break;
			}
		}
	}
}