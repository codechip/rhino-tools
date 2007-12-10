using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binsor.Presentation.Framework.Interfaces;
using System.Collections;

namespace Binsor.Presentation.Framework.Services
{
	public class DefaultLayoutSelector : ILayoutSelector
	{
		IDictionary acceptableViewNamesPerLayout;

		/// <summary>
		/// no config means it will use the default name matching
		/// </summary>
		public DefaultLayoutSelector()
		{

		}

		public DefaultLayoutSelector(IDictionary acceptableViewNames)
		{
			this.acceptableViewNamesPerLayout = acceptableViewNames;
		}

		public bool CanAccept(ILayout layout, IView view)
		{
			IEnumerable<string> acceptableViewNames = (IEnumerable<string>)acceptableViewNamesPerLayout[layout.Name];
			if (acceptableViewNames == null)
				return layout.Name == view.Name;
			return acceptableViewNames.Contains(view.Name);
		}
	}
}
