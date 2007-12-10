using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binsor.Presentation.Framework.Interfaces;

namespace Blog.MainContent
{
	public class MainContentModuleLoader : IModuleLoader
	{
		private IViewMarker<MainContentView> mainContent;

		public MainContentModuleLoader(IViewMarker<MainContentView> mainContent)
		{
			this.mainContent = mainContent;
		}

		public void Initialize(IApplicationContext context, IApplicationShell shell)
		{
			context.Layouts.AddView(mainContent);
		}
	}
}
