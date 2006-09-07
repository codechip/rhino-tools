using Castle.Model;
using NUnit.Framework;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class RhinoContianerTests
    {
        [Test]
        public void CanCreateDependencyWithInstnaceObject()
        {
            RhinoContainer container = new RhinoContainer();
            container.AddComponent("Foo", typeof (Fubar));
            object bar = new object();
        //    container.RegisterDependencyItem(container.Kernel.GetHandler("Foo"), "foo", bar);
            Fubar resolve = container.Resolve<Fubar>();
            Assert.AreSame(bar, resolve.Foo);
        }
    }
}