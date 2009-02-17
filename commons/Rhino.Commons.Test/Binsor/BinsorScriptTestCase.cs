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
using System.IO;
using System.Reflection;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorScriptTestCase : ConfigurationAsserts
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
            base.TestInitialize();

			_container = new WindsorContainer();
		}

		[Test]
		public void CanInstallBinsorScriptFromFile()
		{
			_container.Install(BinsorScript
				.FromFile(Path.GetFullPath(@"Binsor\Windsor2.boo"))
				);

			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test]
		public void CanInstallBinsorInlineScript()
		{
			_container.Install(BinsorScript.Inline(
				@"component 'default_repository', IRepository, NHRepository:
					lifestyle Transient"
				));

			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");				
		}

		[Test]
		public void CanInstallBinsorScriptFromFileAndGenerateAssembly()
		{
			_container.Install(BinsorScript
				.FromFile(Path.GetFullPath(@"Binsor\Windsor2.boo"))
				.GenerateAssembly()
				);

			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test]
		public void CanInstallBinsorScriptFromGeneratedAssembly()
		{
			_container.Install(BinsorScript
				.FromFile(Path.GetFullPath(@"Binsor\Windsor2.boo"))
				.GenerateAssembly()
				);

			IWindsorContainer container = new WindsorContainer()
				.Install(BinsorScript.FromCompiledAssembly("Windsor2.dll")
				);
				
			bool has_repos = container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test, ExpectedArgumentException]
		public void InstallBinsorScriptFromInvalidAssembly_ThrowsException()
		{
			_container.Install(BinsorScript.FromCompiledAssembly(
				Assembly.GetExecutingAssembly())
				);
		}
		
		[Test]
		public void CanInstallBinsorScriptFromRunner()
		{
			AbstractConfigurationRunner runner = BooReader.GetConfigurationInstanceFromFile(
				Path.GetFullPath(@"Binsor\Windsor2.boo"), "", _container,
				BooReader.GenerationOptions.Memory);

			_container.Install(BinsorScript.FromRunner(runner));

			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test, Ignore("Failing")]
		public void CanInstallBinsorScriptWithImportedNamespaces()
		{
			_container.Install(BinsorScript
				.FromFile(Path.GetFullPath(@"Binsor\CustomNamespaces.boo"))
				.ImportNamespaces("Rhino.Commons.Test")
				);

			IRepository<Fubar> fubar_repos = _container.Resolve<IRepository<Fubar>>();
			Assert.IsNotNull(fubar_repos);
		}

		[Test]
		public void CanInstallBinsorScriptFileAndReuseIt()
		{
			BinsorFileInstaller fromFile = BinsorScript
				.FromFile(Path.GetFullPath(@"Binsor\Windsor2.boo"))
				.Reusable();

			_container.Install(fromFile);

			IWindsorContainer container = new WindsorContainer()
				.Install(fromFile);

			bool has_repos = container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}
	}
}

