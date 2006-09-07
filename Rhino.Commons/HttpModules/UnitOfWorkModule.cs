using System;
using System.Web;
using Castle.Windsor;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
    public class UnitOfWorkModule : IHttpModule
    {
        private IWindsorContainer windsorContainer;

        public void Init(HttpApplication context)
        {
            windsorContainer = new RhinoContainer(Settings.Default.WindsorConfig);
            IoC.Initialize(windsorContainer);
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            UnitOfWork.Current.Dispose();
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            IUnitOfWork start = UnitOfWork.Start();
        }

        public void Dispose()
        {
            IoC.Reset(windsorContainer);            
            windsorContainer.Dispose();
        }
    }
}
