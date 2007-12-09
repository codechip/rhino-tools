using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.UI.Facilities
{
	using Castle.MicroKernel.Facilities;
	using Resolvers;

	public class AddCommonResolversFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.Resolver.AddSubResolver(new ArrayOfComponentsResolver(Kernel));
		}
	}
}
