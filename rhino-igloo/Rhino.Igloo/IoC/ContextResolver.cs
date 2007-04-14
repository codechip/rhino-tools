using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Rhino.Commons;
using Rhino.Igloo;

namespace Rhino.Igloo
{
	/// <summary>
	/// Resolver to find the current IContext for components.
	/// Works by finding the context provider and delegating to it.
	/// </summary>
	public class ContextResolver : ISubDependencyResolver
	{
		/// <summary>
		/// Get the current context for the application
		/// </summary>
		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			return IoC.Resolve<IContextProvider>().Current;
		}

		/// <summary>
		/// Returns true if the dependency is IContext
		/// </summary>
		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			return dependency.TargetType == typeof (IContext);
		}
	}
}