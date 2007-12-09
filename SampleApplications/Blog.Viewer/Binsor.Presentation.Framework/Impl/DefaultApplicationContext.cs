namespace Binsor.Presentation.Framework.Impl
{
	using System;
	using System.Collections;
	using Interfaces;
    using System.Windows;

	[Component("ApplicationContext")]
	public class DefaultApplicationContext : IApplicationContext
	{
		private readonly IApplicationShell shell;
		private readonly IModuleLoader[] loaders;

		public DefaultApplicationContext(
			IApplicationShell shell,
			IModuleLoader[] loaders
			)
		{
			this.shell = shell;
			this.loaders = loaders;
		}

		public void Start()
		{
			foreach (IModuleLoader loader in loaders)
			{
				loader.Initialize(this, shell);
			}
		}
	}
}