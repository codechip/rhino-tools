using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for user controls in the application
	/// </summary>
	[View]
    public class BaseControl : UserControl
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseControl"/> class.
		/// </summary>
        public BaseControl()
        {
            Init += new EventHandler(CommonWebUI.WebUI_InjectComponent);
        }
    }
}
