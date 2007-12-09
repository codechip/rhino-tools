namespace Binsor.Presentation.Framework.Interfaces
{
	using Data;

	public interface IApplicationShell
	{
		void AddMenuItems(params MenuItemData[] items);
	}
}