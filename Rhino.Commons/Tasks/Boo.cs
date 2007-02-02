using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Rhino.Commons.Binsor;

namespace Rhino.Commons.Tasks
{
	public class Boo : Task
	{
		private ITaskItem[] files;
		private ITaskItem[] references;
		private ITaskItem outputAssembly;

		public ITaskItem OutputAssembly
		{
			get { return outputAssembly; }
			set { outputAssembly = value; }
		}

		public ITaskItem[] References
		{
			get { return references; }
			set { references = value; }
		}

		public ITaskItem[] Files
		{
			get { return files; }
			set { files = value; }
		}

		public override bool Execute()
		{
			BooCompiler compiler = new BooCompiler();
			if (OutputAssembly == null)
			{
				compiler.Parameters.Pipeline = new CompileToMemory();
			}
			else
			{
				compiler.Parameters.Pipeline = new CompileToFile();
				compiler.Parameters.OutputAssembly = OutputAssembly.ItemSpec;
			}
			compiler.Parameters.OutputType = CompilerOutputType.ConsoleApplication;

			if (files == null || files.Length == 0)
			{
				Log.LogError("Must specify at least one file for the book task");
				return false;
			}

			foreach (ITaskItem taskItem in files)
			{
				FileInput input = new FileInput(taskItem.ItemSpec);
				compiler.Parameters.Input.Add(input);
			}

			foreach (ITaskItem reference in references)
			{
				Assembly assembly = Assembly.LoadFrom(reference.ItemSpec);
				compiler.Parameters.References.Add(assembly);
			}

			CompilerContext run = compiler.Run();

			if (run.Errors.Count > 0)
			{
				string s = run.Errors.ToString(true);
				Log.LogError("Failed to compile code: " + s);
				return false;
			}

			MethodInfo methodInfo = run.GeneratedAssembly.EntryPoint;
			if (methodInfo == null)
			{
				Log.LogError("Could not find entry point for the files");
				return false;
			}
			try
			{
				methodInfo.Invoke(null, new object[] { new string[0] });
			}
			catch (TargetInvocationException e)
			{
				Log.LogError("Scripts failed to run!");
				Log.LogError(e.InnerException.ToString());
			}

			return true;
		}
	}
}
