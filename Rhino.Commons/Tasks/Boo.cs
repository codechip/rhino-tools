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

using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
