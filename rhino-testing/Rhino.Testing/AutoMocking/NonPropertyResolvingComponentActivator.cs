using Castle.Core;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    class NonPropertyResolvingComponentActivator : AutoMockingComponentActivator
    {
        public NonPropertyResolvingComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        protected override void SetUpProperties(object instance, CreationContext context)
        {
            // Do nothing - we're not resolving properties
        }
    }
}
