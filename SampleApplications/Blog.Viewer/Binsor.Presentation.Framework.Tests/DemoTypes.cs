namespace Binsor.Presentation.Framework.Tests.Demo
{
	using System;
    using Binsor.Presentation.Framework.Interfaces;
    using Binsor.Presentation.Framework.Data;

	public class DemoView : IView
	{
	}

	public class DemoLayout : ILayout
	{
		public bool CanAccept(IView view)
		{
			throw new NotImplementedException();
		}

		public void AddView(IView view)
		{
			throw new NotImplementedException();
		}
	}

	public class DemoModuleLoader : IModuleLoader
	{
		private readonly MenuItemData[] items;

		public MenuItemData[] Items
		{
			get { return items; }
		}

		public DemoModuleLoader(MenuItemData[] items)
		{
			this.items = items;
		}

		public void Initialize(IApplicationContext context, IApplicationShell shell)
		{
			throw new NotImplementedException();
		}
	}

	public class DemoPresenter : IPresenter
	{
	}

	public class DemoShellView : IApplicationShell
	{
		public void AddMenuItems(params MenuItemData[] items)
		{
			throw new NotImplementedException();
		}
	}
}