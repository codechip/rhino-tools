using System;
using Castle.MicroKernel.Facilities;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Registers various add-ons to the container
    /// </summary>
    public class AdditionalFunctionalityFacility : AbstractFacility
    {
	    /// <summary>
	    /// The actual addition of resolvers
	    /// </summary>
	    protected override void Init()
	    {   
            /*
             * Add caching support
				Kernel.AddComponent("caching.interceptor", typeof(CachingInterceptor));
				Kernel.ComponentModelBuilder.AddContributor(new CachingInterceptorContributer());
            */
		    Kernel.Resolver.AddSubResolver(new ContextResolver());
		    Kernel.Resolver.AddSubResolver(new LoggingResolver());
	    }
    }
}