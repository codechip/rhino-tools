using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Parser;
using Castle.Windsor;

namespace Rhino.Commons.Binsor
{
	public static class BooReader
	{
		static object BinsorComponents = new object();

		public static ICollection<Component> Components
		{
			get
			{
				ICollection<Component> components = (ICollection<Component>) Local.Data[BinsorComponents];
				if (components == null)
					Local.Data[BinsorComponents] = components = new List<Component>();
				return components;
			}
		}


		private static BooToken tokenThatIsNeededToKeepReferenceToTheBooParserAssembly = new BooToken();

		public static void Read(IWindsorContainer contianer, string fileName)
		{
			try
			{
				using (IoC.UseLocalContainer(contianer))
				{
					IConfigurationRunner conf = GetConfigurationInstanceFromFile(fileName);
					conf.Run();
					foreach (Component component in Components)
					{
						component.Register();
					}
				}
			}
			finally
			{
				Components.Clear();
			}
		}

		private static IConfigurationRunner GetConfigurationInstanceFromFile(string fileName)
		{
			FileInput fileInput = new FileInput(fileName);
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Pipeline = new CompileToMemory();
			compiler.Parameters.Pipeline.Insert(2, new BinsorCompilerStep());
			compiler.Parameters.Pipeline.Replace(typeof (ProcessMethodBodiesWithDuckTyping), new TransformComponentReferences());
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			compiler.Parameters.Input.Add(fileInput);
			compiler.Parameters.References.Add(typeof (BooReader).Assembly);
			CompilerContext run = compiler.Run();
			if (run.Errors.Count != 0)
			{
				throw new CompilerError(string.Format("Could not compile configuration! {0}", run.Errors.ToString(true)));
			}
			Type type = run.GeneratedAssembly.GetType(Path.GetFileNameWithoutExtension(fileName));
			return Activator.CreateInstance(type) as IConfigurationRunner;
		}
	}
}