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
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorTestCase : BaseTest
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
            base.TestInitialize();

			_container = new RhinoContainer();
			string path = Path.GetFullPath(@"Binsor\Windsor.boo");
			Console.WriteLine(path);
			BooReader.Read(_container, path);
		}

		[Test]
		public void CanReadComponentFromConfiguration()
		{
			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test]
		public void CanUseMoreThanSingleFile()
		{
			bool has_disposable = _container.Kernel.HasComponent(typeof(IDisposable));
			Assert.IsTrue(has_disposable, "should have a disposable!");
		}

		[Test]
		public void CanUseSpecilaizedGenerics()
		{
			IRepository<Fubar> resolve = _container.Resolve<IRepository<Fubar>>();
			bool is_instance_of_fake = typeof(FakeRepository<Fubar>).IsInstanceOfType(resolve);
			Assert.IsTrue(is_instance_of_fake);
		}

		[Test]
		public void CanPassComponentReferences()
		{
			FakeRepository<Fubar> fakeRepository = (FakeRepository<Fubar>)_container.Resolve<IRepository<Fubar>>();
			
			Assert.IsNotNull(fakeRepository.Inner);
			Assert.IsTrue(fakeRepository.Inner is NHRepository<Fubar>);
			
		}
		
		[Test]
		public void CanPassPrimitiveParameters()
		{
			EmailSender sender = _container.Resolve<ISender>() as EmailSender;
			Assert.AreEqual("example.dot.org", sender.Host);
		}

		[Test]
		public void CanUseLoops()
		{
			Fubar foo1 = (Fubar)_container.Resolve("foo_1");
			Assert.IsNotNull(foo1);
			Assert.AreEqual(1, foo1.Foo);
			Fubar foo2 = (Fubar)_container.Resolve("foo_2");
			Assert.IsNotNull(foo2);
			Assert.AreEqual(2, foo2.Foo);
			Fubar foo3 = (Fubar)_container.Resolve("foo_3");
			Assert.IsNotNull(foo3);
			Assert.AreEqual(3, foo3.Foo);
		}

		[Test]
		public void CanSpecifyLifeStyle()
		{
			IHandler handler = _container.Kernel.GetHandler("default_repository");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType); 
		}

		[Test]
        public void ContainerUsesXmlConfigurationIfNotBooExtension()
        {
			RhinoContainer container = new RhinoContainer(@"Binsor\Windsor.xml");

			ISender sender = container.Resolve<ISender>();
			Assert.IsNotNull(sender);
        }

		[Test]
		public void CanDefineConfiguration()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender2");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			Assert.AreEqual("component", component.Name);
			Assert.AreEqual("true", component.Attributes["startable"]);

			IConfiguration parameters = AssertChild(component, "parameters");
			IConfiguration to = AssertChild(parameters, "to");
			Assert.AreEqual(2, to.Children.Count);
			Assert.AreEqual("craig", to.Children[0].Value);
			Assert.AreEqual("ayende", to.Children[1].Value);

			IConfiguration backups = AssertChild(parameters, "backups");
			Assert.AreEqual(2, backups.Children.Count);
			Assert.AreEqual("${email_sender}", backups.Children[0].Value);
			Assert.AreEqual("${email_sender}", backups.Children[1].Value);
		}

		[Test]
		public void CanDefineConfigurationsWithKeymap()
		{
			IHandler handler = _container.Kernel.GetHandler("fubar1");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration parameters = AssertChild(component, "parameters");
			IConfiguration fields = AssertChild(parameters, "fields");
			IConfiguration map = AssertChild(fields, "fields");
			Assert.AreEqual(2, map.Children.Count);
			IConfiguration item1 = map.Children[0];
			Assert.AreEqual("name", item1.Attributes["key"]);
			Assert.AreEqual("David Beckham", item1.Value);
			IConfiguration item2 = map.Children[1];
			Assert.AreEqual("age", item2.Attributes["key"]);
			Assert.AreEqual("32", item2.Value);
		}

		[Test]
		public void CanDefineConfigurationsWithKeyvalues()
		{
			IHandler handler = _container.Kernel.GetHandler("fubar2");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration parameters = AssertChild(component, "parameters");
			IConfiguration fields = AssertChild(parameters, "fields");
			Assert.AreEqual(2, fields.Children.Count);
			IConfiguration item1 = fields.Children[0];
			Assert.AreEqual("name", item1.Attributes["key"]);
			Assert.AreEqual("David Beckham", item1.Attributes["value"]);
			IConfiguration item2 = fields.Children[1];
			Assert.AreEqual("age", item2.Attributes["key"]);
			Assert.AreEqual("32", item2.Attributes["value"]);
		}

		[Test]
		public void CanCreateContainerFromEmbeddedResource()
		{
			new RhinoContainer("assembly://Rhino.Commons.Test/Binsor/disposable.boo");
		}

		private static IConfiguration AssertChild(IConfiguration parent, string name)
		{
			IConfiguration config = parent.Children[name];
			Assert.IsNotNull(config);
			return config;
		}
	}
}

