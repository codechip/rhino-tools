namespace Binsor.Presentation.Framework.Services
{
	using System;
	using System.Collections;
	using Interfaces;
    using System.Windows;
	using Binsor.Presentation.Framework.Attributes;

	[Component("ApplicationContext")]
	public class DefaultApplicationContext : IApplicationContext
	{
		private readonly IApplicationShell shell;
		private readonly IModuleLoader[] loaders;
        private readonly ILayoutRegistry layouts;

		public DefaultApplicationContext(
			IApplicationShell shell,
            ILayoutRegistry layouts,
			IModuleLoader[] loaders
			)
		{
			this.shell = shell;
            this.layouts = layouts;
			this.loaders = loaders;
		}

		public void Start()
		{
			foreach (IModuleLoader loader in loaders)
			{
				loader.Initialize(this, shell);
			}
		}

        public ILayoutRegistry Layouts
        {
            get { return layouts; }
        }

    }
}