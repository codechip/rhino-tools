using System;
using System.Web.Mvc;
using MultiTenancy.Web.Context;

namespace MultiTenancy.Web.Infrastructure
{
    public class MultiTenantWebFormViewEngine : IViewEngine
    {
        public IViewEngine CurrentTenantViewEngine
        {
            get
            {
                return TenantContext.Current.ViewEngine;
            }
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName)
        {
            return CurrentTenantViewEngine.FindPartialView(controllerContext, partialViewName);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
        {
            return CurrentTenantViewEngine.FindView(controllerContext, viewName, masterName);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            CurrentTenantViewEngine.ReleaseView(controllerContext, view);
        }
    }
}