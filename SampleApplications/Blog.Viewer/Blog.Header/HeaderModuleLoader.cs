using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binsor.Presentation.Framework.Interfaces;

namespace Blog.Header
{
	public class HeaderModuleLoader : IModuleLoader
	{
		private IViewMarker<HeaderView> header;

		public HeaderModuleLoader(IViewMarker<HeaderView> header)
		{
			this.header = header;
		}

		public void Initialize(IApplicationContext context, IApplicationShell shell)
		{
			context.Layouts.AddView(header);
		}

	}
}
