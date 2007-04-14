using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for web services that are exposed for Ajax consumption
	/// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService, View]
    public class BaseScriptService : WebService
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseScriptService"/> class.
		/// </summary>
        public BaseScriptService()
        {
            CommonWebUI.WebUI_InjectComponent(this, EventArgs.Empty);   
        }
    }
}
