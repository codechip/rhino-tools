namespace IoC.UI.Interfaces
{
	using Data;

	public interface IShellView
	{
		void AddMenuItems(params MenuItemData[] items);
	}
}