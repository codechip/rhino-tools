namespace Binsor.Presentation.Framework.Interfaces
{
	public interface ILayout
	{
		bool CanAccept(IView view);
		void AddView(IView view);
	}
}
