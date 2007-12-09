namespace IoC.UI.Interfaces
{
	public interface ILayout
	{
		bool CanAccept(IView view);
		void AddView(IView view);
	}
}
