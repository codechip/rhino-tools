using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using log4net;

namespace Rhino.Commons.HttpModules
{
    public class PagePerformanceModule : IHttpModule
    {
        static ILog logger = LogManager.GetLogger(typeof (PagePerformanceModule));
        static object PagePerformanceStartKey = new object();

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items[PagePerformanceStartKey] = DateTime.Now;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;
            DateTime startRequest = (DateTime) HttpContext.Current.Items[PagePerformanceStartKey];
            TimeSpan duration = DateTime.Now - startRequest;
            
            ThreadContext.Properties["page_duration"] = duration.ToString();
            ThreadContext.Properties["start_request"] = startRequest;
            ThreadContext.Properties["page_url"] = request.RawUrl;
            
            logger.InfoFormat("Processing {0} started at {1} took {2}", request.RawUrl, 
                              startRequest,duration);

            ResetLoggingProperties();
        }

        private static void ResetLoggingProperties()
        {
            ThreadContext.Properties["page_duration"] = null;
            ThreadContext.Properties["start_request"] = null;
            ThreadContext.Properties["page_url"] = null;
        }

        public void Dispose()
        {
        }
    }
}