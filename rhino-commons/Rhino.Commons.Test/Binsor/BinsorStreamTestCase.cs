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
	public class BinsorStreamTestCase 
	{
		private IWindsorContainer _container;
		
		[SetUp]
		public void TestInitialize()
		{
			_container = new RhinoContainer();

			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Rhino.Commons.Test.Binsor.EmbeddedWindsor.boo");

			BooReader.Read(_container, stream, "EmbdeddedWindsor");
		}

		[Test]
		public void CanReadComponentFromConfiguration()
		{
			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
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

			IConfiguration parameters = AssertConfiguration(component, "parameters");
			IConfiguration to = AssertConfiguration(parameters, "to");
			Assert.AreEqual(2, to.Children.Count);
			Assert.AreEqual("craig", to.Children[0].Value);
			Assert.AreEqual("ayende", to.Children[1].Value);

			IConfiguration backups = AssertConfiguration(parameters, "backups");
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
			IConfiguration parameters = AssertConfiguration(component, "parameters");
			IConfiguration fields = AssertConfiguration(parameters, "fields");
			IConfiguration map = AssertConfiguration(fields, "fields");
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
			IConfiguration parameters = AssertConfiguration(component, "parameters");
			IConfiguration fields = AssertConfiguration(parameters, "fields");
			Assert.AreEqual(2, fields.Children.Count);
			IConfiguration item1 = fields.Children[0];
			Assert.AreEqual("name", item1.Attributes["key"]);
			Assert.AreEqual("David Beckham", item1.Attributes["value"]);
			IConfiguration item2 = fields.Children[1];
			Assert.AreEqual("age", item2.Attributes["key"]);
			Assert.AreEqual("32", item2.Attributes["value"]);
		}

		private static IConfiguration AssertConfiguration(IConfiguration parent, string name)
		{
			IConfiguration config = parent.Children[name];
			Assert.IsNotNull(config);
			return config;
		}
	}
}
