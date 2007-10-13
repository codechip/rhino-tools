using System;
using System.Collections.Generic;

namespace Rhino.Commons.Boo
{
	using System.IO;
	using System.Reflection;
	using global::Boo.Lang.Compiler;
	using global::Boo.Lang.Compiler.Ast;
	using global::Boo.Lang.Compiler.IO;
	using global::Boo.Lang.Compiler.MetaProgramming;
	using global::Boo.Lang.Compiler.Steps;

	[CLSCompliant(false)]
	public class AutoReferenceFilesCompilerStep : AbstractTransformerCompilerStep
	{
		private readonly Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();

		private readonly string baseDirectory;

		public AutoReferenceFilesCompilerStep(string baseDirectory)
		{
			this.baseDirectory = baseDirectory;
		}

		public override void OnImport(Import node)
		{
			if (node.Namespace != "file")
				return;

			RemoveCurrentNode();

			//we may need to preserve this, since it may be used in several compiler cycles.
			//which will set them to different things
			CompilerErrorCollection errors = Errors;
			AssemblyCollection references = Parameters.References;
			string path = node.AssemblyReference.Name
				.Replace("~", AppDomain.CurrentDomain.BaseDirectory);
			path = Path.Combine(baseDirectory, path);

			Assembly assembly;
			if (assemblyCache.TryGetValue(path, out assembly) == false)
			{
				assembly = CompileAssembly(node, path, errors);

				if (assembly == null)
				{
					throw new CompilationErrorsException(errors);
				}

				assemblyCache.Add(path, assembly);
			}

			references.Add(assembly);
		}

		private Assembly CompileAssembly(Node node, string path, CompilerErrorCollection errors)
		{
			CompilerContext result = Compile(path);
			if (result.Errors.Count > 0)
			{
				errors.Add(new CompilerError(node.LexicalInfo, "Failed to add a file reference"));
				foreach (CompilerError err in result.Errors)
				{
					errors.Add(err);
				}
				return null;
			}
			return result.GeneratedAssembly;
		}

		private CompilerContext Compile(string input)
		{
			CompilerParameters parameters = SafeCloneParameters(Parameters);
			parameters.Input.Add(new FileInput(input));
			BooCompiler compiler = new BooCompiler(parameters);
			return compiler.Run();
		}

		/// <summary>
		/// This creates a copy of the passed compiler parameters, without the stuff
		/// that make a compilation unique, like input, output assembly, etc
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static CompilerParameters SafeCloneParameters(CompilerParameters parameters)
		{
			CompilerParameters cloned = new CompilerParameters();
			cloned.BooAssembly = parameters.BooAssembly;
			cloned.Checked = parameters.Checked;
			cloned.Debug = parameters.Debug;
			cloned.DelaySign = parameters.DelaySign;
			cloned.Ducky = parameters.Ducky;
			cloned.GenerateInMemory = parameters.GenerateInMemory;

			// cloned.Input - we don't want to copy that
			cloned.KeyContainer = parameters.KeyContainer;
			cloned.KeyFile = parameters.KeyFile;
			cloned.LibPaths.AddRange(parameters.LibPaths);
			cloned.MaxAttributeSteps = parameters.MaxAttributeSteps;
			// cloned.OutputAssembly - we don't want that either

			// always want that, since we are compiling to add a reference
			cloned.OutputType = CompilerOutputType.Library;
			cloned.OutputWriter = parameters.OutputWriter;
			cloned.Pipeline = parameters.Pipeline;
			cloned.References = parameters.References;
			// cloned.Resources - probably won't have that, but in any case, not relevant
			cloned.StdLib = parameters.StdLib;

			return cloned;
		}

		public override void Run()
		{
			Visit(CompileUnit);
		}
	}
}