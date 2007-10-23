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
using System.Collections.Generic;
using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Parser;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Core.Resource;
using Castle.Windsor;
using Rhino.Commons.Boo;

namespace Rhino.Commons.Binsor
{
	public static class BooReader
    {
        private static readonly BooToken tokenThatIsNeededToKeepReferenceToTheBooParserAssembly = new BooToken();

        internal static ICollection<INeedSecondPassRegistration> NeedSecondPassRegistrations
        {
            get
            {
                ICollection<INeedSecondPassRegistration> data = (ICollection<INeedSecondPassRegistration>)
                    Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly];
                if (data == null)
                {
                    Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly] =
                        data = new List<INeedSecondPassRegistration>();
                }
                return data;
            }
            set
            {
                Local.Data[tokenThatIsNeededToKeepReferenceToTheBooParserAssembly] = value;
            }
        }

        public static void Read(IWindsorContainer container, string fileName)
        {
            Read(container, fileName, GenerationOptions.Memory);
        }

        public static void Read(IWindsorContainer container, string fileName, GenerationOptions generationOptions)
        {
            try
            {
                using (IoC.UseLocalContainer(container))
                {
                    IConfigurationRunner conf = GetConfigurationInstanceFromFile(
						fileName, container, generationOptions);
                    conf.Run();
                    foreach (INeedSecondPassRegistration needSecondPassRegistration in NeedSecondPassRegistrations)
                    {
                        needSecondPassRegistration.RegisterSecondPass();
                    }
                }
            }
            finally
            {
                NeedSecondPassRegistrations = null;
            }
        }

		public static void Read(IWindsorContainer container, Stream stream, string name)
		{
			Read(container, stream, GenerationOptions.Memory, name);
		}

		public static void Read(IWindsorContainer container, Stream stream, GenerationOptions generationOptions, string name)
		{
			try
			{
				using (IoC.UseLocalContainer(container))
				{
					IConfigurationRunner conf = GetConfigurationInstanceFromStream(
						name, container, stream, generationOptions);
					conf.Run();
					foreach (INeedSecondPassRegistration needSecondPassRegistration in NeedSecondPassRegistrations)
					{
						needSecondPassRegistration.RegisterSecondPass();
					}
				}
			}
			finally
			{
				NeedSecondPassRegistrations = null;
			}
		}


		private static IConfigurationRunner GetConfigurationInstanceFromFile(
			string fileName, IWindsorContainer container, GenerationOptions generationOptions)
		{
			string baseDirectory = Path.GetDirectoryName(fileName);
			UrlResolverDelegate urlResolver = CreateWindorUrlResolver(container);
			return GetConfigurationInstance(Path.GetFileNameWithoutExtension(fileName),
											new FileInput(fileName), generationOptions,
											new AutoReferenceFilesCompilerStep(baseDirectory, urlResolver));
        }

		private static IConfigurationRunner GetConfigurationInstanceFromStream(
			string name, IWindsorContainer container, Stream stream, GenerationOptions generationOptions)
		{
			UrlResolverDelegate urlResolver = CreateWindorUrlResolver(container);
			return GetConfigurationInstance(name, new ReaderInput(name, new StreamReader(stream)),
			                                generationOptions, new AutoReferenceFilesCompilerStep(urlResolver));
		}

		private static IConfigurationRunner GetConfigurationInstance(
			string name, ICompilerInput input, GenerationOptions generationOptions,
			AutoReferenceFilesCompilerStep autoReferenceStep)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			if (generationOptions == GenerationOptions.Memory)
				compiler.Parameters.Pipeline = new CompileToMemory();
			else
				compiler.Parameters.Pipeline = new CompileToFile();

			compiler.Parameters.Pipeline.Insert(1, autoReferenceStep);
			compiler.Parameters.Pipeline.Insert(2, new BinsorCompilerStep());
			compiler.Parameters.Pipeline.Replace(
				typeof(ProcessMethodBodiesWithDuckTyping),
				new TransformUnknownReferences());
			compiler.Parameters.Pipeline.InsertAfter(typeof(TransformUnknownReferences),
				new RegisterComponentAndFacilitiesAfterCreation());
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			compiler.Parameters.Input.Add(input);
			compiler.Parameters.References.Add(typeof(BooReader).Assembly);
			CompilerContext run = compiler.Run();
			if (run.Errors.Count != 0)
			{
				throw new CompilerError(string.Format("Could not compile configuration! {0}", run.Errors.ToString(true)));
			}
			Type type = run.GeneratedAssembly.GetType(name);
			return Activator.CreateInstance(type) as IConfigurationRunner;
		}

		private static UrlResolverDelegate CreateWindorUrlResolver(IWindsorContainer container)
		{
			IResourceSubSystem subSystem = (IResourceSubSystem)
					container.Kernel.GetSubSystem(SubSystemConstants.ResourceKey);

			return delegate(string url, string basePath)
			       {
			       	IResource resource;

					if (url.IndexOf(':') < 0)
					{
						url = "file://" + url;
					}

			       	if (!string.IsNullOrEmpty(basePath))
			       	{
			       		resource = subSystem.CreateResource(url, basePath);
			       	}
			       	else
			       	{
			       		resource = subSystem.CreateResource(url);
			       	}
			       	return resource.GetStreamReader();
			       };
		}

        public enum GenerationOptions
        {
            Memory,
            File
        }
    }
}
