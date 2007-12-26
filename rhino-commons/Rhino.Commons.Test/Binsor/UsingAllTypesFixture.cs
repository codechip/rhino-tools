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
        public void CanGetAllTypesByAttribute()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UsingAllTypes.boo");
            Assert.IsNotNull(container[typeof(Controller)]);
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
}