using System;
using System.Web;
using System.Web.Mvc;
using MultiTenancy.Web.Context;

namespace MultiTenancy.Web.Infrastructure
{
    public class MultiTenantControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            controllerName = controllerName.ToLowerInvariant();
            var context = TenantContext.Current;
            if (context.Container.Kernel.HasComponent(controllerName)==false)
                throw new HttpException(404, "Could not find appropriate controller");

            return (IController) context.Container.Resolve(controllerName);
        }

        public override void ReleaseController(IController controller)
        {
            TenantContext.Current.Container.Release(controller);
        }
    }
}