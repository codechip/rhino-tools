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

using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Installer;
using System.Collections.Generic;

namespace Rhino.Commons.Binsor
{
	public abstract class BinsorScriptInstaller<T> : IWindsorInstaller
		where T : BinsorScriptInstaller<T>
	{
		private EnvironmentDelegate environment;
		private BooReader.GenerationOptions options;
		private List<string> namespaces;
		private AbstractConfigurationRunner runner;
		private bool reusable;
		
		public BinsorScriptInstaller()
		{
			options = BooReader.GenerationOptions.Memory;
			namespaces = new List<string>();
		}
		
		public T GenerateAssembly()
		{
			options = BooReader.GenerationOptions.File;
			return (T)this;
		}
		
		public T Environment(EnvironmentDelegate environment)
		{
			this.environment = environment;
			return (T)this;
		}

		public T ImportNamespaces(params string[] namespaces)
		{
			this.namespaces.AddRange(namespaces);
			return (T)this;
		}

		public T Reusable()
		{
			reusable = true;
			return (T)this;
		}

		protected BooReader.GenerationOptions GenerationOptions
		{
			get { return options; }
		}
		
		protected string EnvironmentName
		{
			get
			{
				return ( environment != null ) ? environment() : null;
			}
		}

		protected string[] ImportedNamespaces
		{
			get { return namespaces.ToArray(); }
		}

		#region IWindsorInstaller Members

		void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
		{
			if (reusable && runner != null)
			{
				BinsorScript.FromRunner(runner).InstallInto(container);
			}
			else
			{
				runner = InstallInto(container);
			}
		}

		#endregion

		protected abstract AbstractConfigurationRunner InstallInto(IWindsorContainer container);
	}
}
