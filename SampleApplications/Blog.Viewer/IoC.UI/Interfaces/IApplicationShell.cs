namespace IoC.UI.Interfaces
{
	using Data;

	public interface IApplicationShell
	{
		void AddMenuItems(params MenuItemData[] items);
	}
}