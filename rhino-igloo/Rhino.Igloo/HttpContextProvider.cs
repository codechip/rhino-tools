using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using Rhino.Igloo;
using Rhino.Igloo.Properties;

namespace Rhino.Igloo
{
	/// <summary>
	/// Exposes the current operation context
	/// </summary>
	public class HttpContextProvider : IContextProvider
	{
		/// <summary>
		/// The current HTTP Context
		/// </summary>
		public IContext Current
		{
			get
			{
				if (HttpContext.Current == null)
					return new ThrowingContextAdapter();
				return new HttpContextAdapter(HttpContext.Current);
			}
		}
	}
}
