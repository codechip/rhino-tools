namespace Binsor.Presentation.Framework.Tests
{
	using System.Threading;
	using System.Windows;
	using MbUnit.Framework;
	using Binsor.Presentation.Framework.Services;
	using Binsor.Presentation.Framework.Tests.Demo;
	using System.Windows.Controls;
	using Binsor.Presentation.Framework.Layouts;

	[TestFixture(ApartmentState = ApartmentState.STA)]
	public class PanelDecoratingLayoutFixture
	{
		[Test]
		public void When_adding_a_view_to_the_layout_should_add_view_to_panel_children_collection()
		{
			Panel element = new DockPanel();
			PanelDecoratingLayout layout = new PanelDecoratingLayout(element);
			layout.AddView(new DemoView());
			Assert.AreEqual(1, element.Children.Count);
		}
	}
}