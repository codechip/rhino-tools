using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for the master page.
	/// </summary>
	[View]
    public abstract class BaseMaster : MasterPage
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseMaster"/> class.
		/// </summary>
        public BaseMaster()
        {
            Init+=new EventHandler(CommonWebUI.WebUI_InjectComponent);
        }

        /// <summary>
        /// Gets the script manager.
        /// </summary>
        /// <value>The script manager.</value>
        public abstract ScriptManager ScriptManager
        {
            get; 
        }
    }
}
