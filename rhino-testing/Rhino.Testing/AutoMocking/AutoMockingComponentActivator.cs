using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;

namespace Rhino.Testing.AutoMocking
{
	public class AutoMockingComponentActivator : DefaultComponentActivator
	{
		public AutoMockingComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation,
											  ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override void SetUpProperties(object instance, CreationContext context)
		{

		}
	}
}
