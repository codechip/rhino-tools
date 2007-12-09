namespace Blog.Shell
{
	using IoC.UI.Data;
	using IoC.UI.Interfaces;

	public class ShellModuleLoader : IModuleLoader
	{
		private readonly MenuItemData[] menuItemDatas;

		public ShellModuleLoader(MenuItemData[] menuItemDatas)
		{
			this.menuItemDatas = menuItemDatas;
		}

		public void Initialize(IApplicationContext context, IApplicationShell shell)
		{
			shell.AddMenuItems(menuItemDatas);
		}
	}
}