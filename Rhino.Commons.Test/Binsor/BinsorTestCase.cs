using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using NUnit.Framework;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorTestCase
	{
		[Test]
		public void CanReadComponentFromConfiguration()
		{
			IWindsorContainer container = new RhinoContainer();
			BooReader.Read(container, @"..\..\Binsor\Windsor.boo");
			bool has_repos = container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");

			IRepository<Fubar> resolve = container.Resolve<IRepository<Fubar>>();

			bool is_instance_of_fake = typeof(FakeRepository<Fubar>).IsInstanceOfType(resolve);
			
			Assert.IsTrue(is_instance_of_fake);

			EmailSender sender = container.Resolve<ISender>() as EmailSender;

			Assert.AreEqual("example.dot.org", sender.Host);
		}
	}
}
