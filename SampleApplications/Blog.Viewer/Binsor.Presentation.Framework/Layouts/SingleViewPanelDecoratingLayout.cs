namespace Binsor.Presentation.Framework.Layouts
{
	using System.Windows.Controls;
	using Attributes;
	using Interfaces;

	[SkipAutomaticRegistration("Layouts are configured manually, because they are tied to specific elements.")]
	public class SingleViewPanelDecoratingLayout : PanelDecoratingLayout
	{
		public SingleViewPanelDecoratingLayout(Panel element, string[] acceptableViewNames) : base(element, acceptableViewNames)
		{ 
		}

		public override void AddView(IView view)
		{
			Element.Children.Clear();
			base.AddView(view);
		}
	}
}