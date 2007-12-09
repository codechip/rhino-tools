namespace Binsor.Presentation.Framework.Interfaces
{
	public interface ILayout
	{
		string Name { get; }
		bool CanAccept(IView view);
		void AddView(IView view);
	}
}
