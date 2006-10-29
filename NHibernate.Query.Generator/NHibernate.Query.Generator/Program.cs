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
		private static string targetExtention;
		private static string outputDir;
		private static CodeDomProvider provider = null;

		static void Main(string[] args)
		{
			string inputFilePattern = GetCommandLineArguments(args);
			try
			{
				SetupCodeProvider();
				string directoryName = Path.GetDirectoryName(inputFilePattern);
				if (string.IsNullOrEmpty(directoryName))
					directoryName = ".";
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
			string fileExt = Path.GetExtension(file);
			string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) +"."+ targetExtention);
			// hbm file
			if (fileExt.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase))
			{
				GenerateSingleFile(File.OpenText(file), outputFile);
			}
			else if (fileExt.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) ||
			         fileExt.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase))// Active Record...
			{
				GenerateFromActiveRecordAssembly(file);
			}
		}

		private static void OutputQueryBuilder()
		{
//write query builders so user can just include the whole directory.
			Stream namedExp = typeof(Program).Assembly.GetManifestResourceStream("NHibernate.Query.Generator.QueryBuilders.QueryBuilder."+targetExtention);
			File.WriteAllText(Path.Combine(outputDir, "QueryBuilder." + targetExtention), new StreamReader(namedExp).ReadToEnd());
			Console.WriteLine("Successfuly created file: {0}\\QueryBuilder.{1}", outputDir, targetExtention);
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
				string genFile = Path.Combine(outputDir, model.Type.Name + "." + targetExtention);
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
			targetExtention = args[0];
			outputDir = args[2];
			return args[1];
		}

		private static void SetupCodeProvider()
		{
			switch (targetExtention.ToLower())
			{
				case "vb":
					provider = new VBCodeProvider();
					break;
				case "cs":
					provider = new CSharpCodeProvider();
					break;
				default:
					Console.WriteLine("Unknown language element, expected 'cs' or 'vb', but got {0}", targetExtention);
					Environment.Exit(1);
					break;
			}
		}
	}
}