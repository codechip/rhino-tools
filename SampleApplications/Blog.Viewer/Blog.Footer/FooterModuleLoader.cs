using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binsor.Presentation.Framework.Interfaces;

namespace Blog.Footer
{
	public class FooterModuleLoader : IModuleLoader
	{
		private IViewMarker<FooterView> footer;

		public FooterModuleLoader(IViewMarker<FooterView> footer)
		{
			this.footer = footer;
		}

		public void Initialize(IApplicationContext context, IApplicationShell shell)
		{
			context.Layouts.AddView(footer);
		}
	}
}
