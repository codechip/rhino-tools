namespace Binsor.Presentation.Framework.Interfaces
{
	using System.Windows;

	public interface ILayoutDecoratorResolver
	{
		ILayout GetLayoutDecoratorFor(FrameworkElement element);	
	}
}