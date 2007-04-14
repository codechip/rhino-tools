using System;
using Castle.Core;
using Castle.MicroKernel;
using log4net;

namespace Rhino.Igloo
{
	/// <summary>
	/// Resolve a logger to the current class
	/// </summary>
	public class LoggingResolver : ISubDependencyResolver
	{
		/// <summary>
		/// Resolve to a logger for the class that we are currently building.
		/// </summary>
		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			return LogManager.GetLogger(model.Implementation);
		}

		/// <summary>
		/// Returns true if the dependency is of type ILog
		/// </summary>
		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			return (dependency.TargetType == typeof (ILog));
		}
	}
}
