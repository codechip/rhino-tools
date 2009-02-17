using System;
using System.Reflection;
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

        protected override object CreateInstance(CreationContext context, object[] arguments, System.Type[] signature)
        {
            // TODO: Support interceptors + copy "use fast create instance" logic from DefaultComponentActivator

            // Support internal and private constructors
            return Activator.CreateInstance(Model.Implementation,
                                            BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public |
                                            BindingFlags.Instance, null, arguments, null, null);
        }
	}
}
