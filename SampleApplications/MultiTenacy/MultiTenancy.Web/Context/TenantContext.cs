using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Castle.Windsor;

namespace MultiTenancy.Web.Context
{
    public class TenantContext
    {
        private static readonly RootContext rootContext = new RootContext();
        private static IDictionary<string, TenantContext> Contexts = new Dictionary<string, TenantContext>();

        public static Func<string> CurrentTenantId = () => "MultiTenancy.Web";
        private readonly string assembly;

        private readonly WebFormViewEngine viewEngine;

        public TenantContext(IWindsorContainer container, string assembly)
        {
            this.assembly = assembly;
            Container = container;

            viewEngine = new WebFormViewEngine
            {
                MasterLocationFormats = new[]
                {
                    "~/Tenants/" + this.assembly+ "/{1}/{0}.master",
                    "~/Tenants" + this.assembly + "/Shared/{0}.master",
                    "~/Views/{1}/{0}.master",
                    "~/Views/Shared/{0}.master"
                },
                ViewLocationFormats = new[]
                {
                    "~/Tenants/" + this.assembly+ "/{1}/{0}.aspx",
                    "~/Tenants/" + this.assembly+ "/{1}/{0}.ascx",
                    "~/Tenants/" + this.assembly+ "/Shared/{0}.aspx",
                    "~/Tenants/" + this.assembly+ "/Shared/{0}.ascx",
                    "~/Views/{1}/{0}.aspx",
                    "~/Views/{1}/{0}.ascx",
                    "~/Views/Shared/{0}.aspx",
                    "~/Views/Shared/{0}.ascx"
                }
            };
            
        }

        public IWindsorContainer Container { get; private set; }

        public static TenantContext Current
        {
            get
            {
                string currentTenantId = CurrentTenantId();
                TenantContext value;
                if (Contexts.TryGetValue(currentTenantId, out value))
                    return value;

                lock (typeof (TenantContext))
                {
                    if (Contexts.TryGetValue(currentTenantId, out value))
                        return value;

                    value = CreateContext(currentTenantId);
                    Contexts = new Dictionary<string, TenantContext>(Contexts)
                    {
                        {currentTenantId, value}
                    };
                    return value;
                }
            }
        }

        public IViewEngine ViewEngine
        {
            get { return viewEngine; }
        }

        private static TenantContext CreateContext(string currentTenantAssembly)
        {
            Assembly assembly = Assembly.Load(currentTenantAssembly);
            Type bootStrapper = assembly.GetType(assembly.GetName().Name + ".BootStrapper");
            var strapper = (IBootStrapper) Activator.CreateInstance(bootStrapper);
            strapper.Init(rootContext);
            return strapper.CreateContext();
        }
    }
}