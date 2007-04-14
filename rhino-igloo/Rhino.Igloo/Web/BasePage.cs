using System;
using System.Web;
using System.Web.UI;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for pages
	/// </summary>
	[View]
    public class BasePage : Page
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BasePage"/> class.
		/// </summary>
        public BasePage()
        {
            Init+=new EventHandler(CommonWebUI.WebUI_InjectComponent);
        }

        /// <summary>
        /// Adds the service reference.
        /// </summary>
        /// <param myName="path">The path.</param>
        protected void AddServiceReference(string path)
        {
            ServiceReference reference = new ServiceReference(path);
            ((BaseMaster)Master).ScriptManager.Services.Add(reference);
        }

        /// <summary>
        /// Gets the services path.
        /// </summary>
        /// <value>The services path.</value>
	    public string ServicesPath
	    {
	        get
	        {
	            return Request.ApplicationPath + "/Services";
	        }
	    }
    }
}
