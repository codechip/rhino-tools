namespace IoC.UI.Resolvers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.MicroKernel;

	public class ArrayOfComponentsResolver : ISubDependencyResolver
	{
		private readonly IKernel kernel;

		public ArrayOfComponentsResolver(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
							  DependencyModel dependency)
		{
			Type elementType = dependency.TargetType.GetElementType();
			Array all = kernel.ResolveAll(elementType, new Hashtable());
			return all;
		}

		public bool CanResolve(CreationContext context,
			ISubDependencyResolver parentResolver,
			ComponentModel model,
			DependencyModel dependency)
		{
			if (dependency.TargetType.IsArray == false)
				return false;
			Type elementType = dependency.TargetType.GetElementType();

			return kernel.HasComponent(elementType);
		}
	}
}
