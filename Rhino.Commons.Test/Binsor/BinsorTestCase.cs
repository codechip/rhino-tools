using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorTestCase
	{
		private IWindsorContainer _container;

		[SetUp]
		public void TestInitialize()
		{
			_container = new RhinoContainer();
			BooReader.Read(_container, @"..\..\Binsor\Windsor.boo");
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
			IHandler handler = _container.Kernel.GetHandler("defualt_repository");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType); 
			
		}
	}
}

