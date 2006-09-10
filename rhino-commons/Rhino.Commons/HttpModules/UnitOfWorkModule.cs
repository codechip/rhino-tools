using System;
using System.Runtime.CompilerServices;
using System.Web;
using Castle.Windsor;
using log4net;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
    public class UnitOfWorkModule : IHttpModule
    {
        static ILog logger = LogManager.GetLogger(typeof (UnitOfWorkModule));
        private IWindsorContainer windsorContainer;
        HttpApplication application;

        public void Init(HttpApplication context)
        {
            application = context;
            logger.Info("Starting Unit Of Work Module");
            InitializeContainer();
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitializeContainer()
        {
            if(IoC.IsInitialized)
                return;
            windsorContainer = new RhinoContainer(Settings.Default.WindsorConfig);
            IoC.Initialize(windsorContainer);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            if(IoC.IsInitialized==false)
                InitializeContainer();
            logger.Debug("Starting Unit Of Work For Request");
            UnitOfWork.Start();
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            logger.Debug("Disposing Unit Of Work For Request");
            UnitOfWork.Current.Dispose();
        }

        public void Dispose()
        {
            application.BeginRequest -= new EventHandler(context_BeginRequest);
            application.EndRequest -= new EventHandler(context_EndRequest);
            logger.Info("Disposing Unit Of Work Module");
            IoC.Reset(windsorContainer);            
            windsorContainer.Dispose();
        }
    }
}
