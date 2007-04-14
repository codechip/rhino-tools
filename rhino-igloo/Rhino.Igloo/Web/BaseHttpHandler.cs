using System;
using System.Web;

namespace Rhino.Igloo
{
    /// <summary>
    /// Base class for HttpHandlers
    /// </summary>
    [View]
    public abstract class BaseHttpHandler : IHttpHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHttpHandler"/> class.
        /// </summary>
        public BaseHttpHandler()
        {
            CommonWebUI.WebUI_InjectComponent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public abstract void ProcessRequest(HttpContext context);

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}