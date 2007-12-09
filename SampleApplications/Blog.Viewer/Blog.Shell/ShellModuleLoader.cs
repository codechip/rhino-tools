namespace Blog.Shell
{
	using Binsor.Presentation.Framework.Data;
	using Binsor.Presentation.Framework.Interfaces;

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