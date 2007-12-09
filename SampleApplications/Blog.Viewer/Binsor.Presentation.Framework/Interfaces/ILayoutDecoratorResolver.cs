namespace Binsor.Presentation.Framework.Interfaces
{
	using System.Windows;

	public interface ILayoutDecoratorResolver
	{
		ILayout GetLayoutDecoratorFor<T>(T element) where T : FrameworkElement;	
	}
}