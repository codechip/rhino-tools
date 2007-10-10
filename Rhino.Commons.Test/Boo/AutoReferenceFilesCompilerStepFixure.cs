namespace Rhino.Commons.Test.Boo
{
	using System;
	using System.IO;
	using System.Reflection;
	using global::Boo.Lang.Compiler;
	using global::Boo.Lang.Compiler.IO;
	using global::Boo.Lang.Compiler.Pipelines;
	using MbUnit.Framework;
	using Rhino.Commons.Boo;

	[TestFixture]
	public class AutoReferenceFilesCompilerStepFixure
	{
		[Test]
		public void CanAddFileReference()
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"boo\hasReferenceToAnotherFile.boo");
			Assembly asm = Compile(path);
			StringWriter sw = new StringWriter();
			TextWriter old = Console.Out;
			try
			{
				Console.SetOut(sw);
				asm.EntryPoint.Invoke(null, new object[1]{null});

				Assert.AreEqual("From second file\r\n", sw.GetStringBuilder().ToString());
			}
			finally
			{
				Console.SetOut(old);
			}
		}

		private static Assembly Compile(string filename)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.OutputType = CompilerOutputType.ConsoleApplication;
			compiler.Parameters.GenerateInMemory = true;
			compiler.Parameters.Pipeline = new CompileToMemory();
			compiler.Parameters.Pipeline.Insert(1, new AutoReferenceFilesCompilerStep(Path.GetDirectoryName(filename)));
			compiler.Parameters.Input.Add(new FileInput(filename));

			CompilerContext run = compiler.Run();
			if (run.Errors.Count > 0)
				throw new CompilerError(run.Errors.ToString(true));
			return run.GeneratedAssembly;
		}
	}
}
