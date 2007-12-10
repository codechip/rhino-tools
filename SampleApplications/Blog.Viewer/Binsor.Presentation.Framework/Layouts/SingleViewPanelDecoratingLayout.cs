namespace Binsor.Presentation.Framework.Layouts
{
	using System.Windows.Controls;
	using Attributes;
	using Interfaces;

	[SkipRegistrationForLayout]
	public class SingleViewPanelDecoratingLayout : PanelDecoratingLayout
	{
		public SingleViewPanelDecoratingLayout(Panel element) : base(element)
		{
		}

		public override void AddView(IView view)
		{
			Element.Children.Clear();
			base.AddView(view);
		}
	}
}