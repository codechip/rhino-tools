using System;
using System.Web.Services;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for ASMX web services
	/// </summary>
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[View]
	public class BaseWebService : WebService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseWebService"/> class.
		/// </summary>
		public BaseWebService()
		{
			CommonWebUI.WebUI_InjectComponent(this, EventArgs.Empty);
		}
	}
}