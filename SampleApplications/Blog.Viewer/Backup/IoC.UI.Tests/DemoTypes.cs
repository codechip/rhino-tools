namespace IoC.UI.Tests.Demo
{
	using System;
	using Data;
	using Interfaces;

	public class DemoView : IView
	{
	}

	public class DemoLayout : ILayout
	{
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

		public void Initialize(IApplicationContext context, IShellView shell)
		{
			throw new NotImplementedException();
		}
	}

	public class DemoPresenter : IPresenter
	{
	}

	public class DemoShellView : IShellView
	{
		public void AddMenuItems(params MenuItemData[] items)
		{
			throw new NotImplementedException();
		}
	}
}