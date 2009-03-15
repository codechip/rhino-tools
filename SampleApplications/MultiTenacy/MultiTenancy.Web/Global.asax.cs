using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MultiTenancy.Web.Context;
using MultiTenancy.Web.Infrastructure;

namespace MultiTenancy.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            TenantContext.CurrentTenantId = 
                () => HttpContext.Current.Request["tenant"] ?? "MultiTenancy.Web";

            ControllerBuilder.Current.SetControllerFactory(
                new MultiTenantControllerFactory());
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MultiTenantWebFormViewEngine());
            

            RegisterRoutes(RouteTable.Routes);
        }
    }
}