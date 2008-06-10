using Castle.MicroKernel;

namespace Rhino.Commons.Test.Binsor
{
    using System;
    using MbUnit.Framework;

    [TestFixture]
    public class UsingAllTypesFixture
    {
        [Test]
        public void CanGetAllTypesByBaseType()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
            Assert.IsNotNull(container[typeof(MyView)]);
        }

        [Test]
        public void CanGetAllTypesByNamespace()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
            Assert.IsNotNull(container[typeof(UsingAllTypesFixture)]);
        }


        [Test]
        public void CanGetAllTypesByAttribute()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
            Assert.IsNotNull(container[typeof(Controller)]);
        }

        [Test]
        public void CanGetAllTypesByPredicate()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
            IHandler handler = container.Kernel.GetHandler("nh.repos");
            Assert.IsNotNull(handler);
            Assert.AreEqual(typeof(IRepository<>), handler.ComponentModel.Service.GetGenericTypeDefinition());
            Assert.AreEqual(typeof(NHRepository<>), handler.ComponentModel.Implementation);
        }

		[Test]
		public void CanGetInterfaceByPredicate()
		{
			RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
			IHandler handler = container.Kernel.GetHandler("foo54");
			Assert.IsNotNull(handler);
			Assert.AreEqual(typeof(IFoo54), handler.ComponentModel.Service);
			Assert.AreEqual(typeof(Fubar54), handler.ComponentModel.Implementation);
		}
    }

    public class ControllerAttribute : Attribute
    {
        
    }

    [Controller]
    public class Controller{}
    public interface IView
    {
        
    }

    public class MyView : IView
    {
        
    }

	public interface IFoo54 { }
	public interface IBar54 { }

	public class Fubar54 : IFoo54, IBar54
	{
		
	}
}
