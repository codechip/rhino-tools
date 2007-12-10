using System.Windows;
namespace Binsor.Presentation.Framework.Interfaces
{
	public interface ILayout
	{
		string Name { get; }
		void AddView(IView view);
	}
}
