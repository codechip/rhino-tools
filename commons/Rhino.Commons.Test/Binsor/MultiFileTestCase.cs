namespace Rhino.Commons.Test.Binsor
{
	using System;
	using System.IO;
	using Castle.Core.Configuration;
	using Castle.MicroKernel;
	using Castle.Windsor;
	using MbUnit.Framework;

	[TestFixture]
	public class MultiFileTestCase : ConfigurationAsserts
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
            base.TestInitialize();

			string path = Path.GetFullPath(@"Binsor\MultiFileConfig_Extension.boo");
			Console.WriteLine(path);
			_container = new RhinoContainer(path);
		}

		[Test]
		public void CanSetAttributeOnComponentDefinedInAnotherFile()
		{
			EmailSender sender = (EmailSender)_container.Resolve<ISender>("email_sender");
			Assert.AreEqual("example.dot.org", sender.Host);
			CollectionAssert.AreElementsEqual(new string[]{"Kaitlyn", "Matthew", "Lauren"},
				sender.To);
		}

		[Test]
		public void CanOverrideAttributesSetOnAnotherFile()
		{
			EmailSender sender = (EmailSender)_container.Resolve<ISender>("email_sender2");
			Assert.AreEqual("example.dot.org", sender.Host);
			CollectionAssert.AreElementsEqual(new string[] { "Kaitlyn", "Matthew", "Lauren" },
				sender.To);
		}

		[Test]
		public void CanSetConfgurationOnComponentDefinedInAnotherFile()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			AssertAttribute(component, "tag1", "important");
			AssertAttribute(component, "tag2", "priority");
		}

		[Test]
		public void CanDefineConfigurationsWithListFromAnotherFile()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration hosts = AssertChild(component, "hosts");
			Assert.AreEqual(2, hosts.Children.Count);
			foreach (IConfiguration child in hosts.Children)
			{
				Assert.AreEqual("host", child.Name);
			}
		}
	}
}