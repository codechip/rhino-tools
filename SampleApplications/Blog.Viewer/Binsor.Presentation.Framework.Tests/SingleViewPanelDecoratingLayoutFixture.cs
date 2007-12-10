namespace Binsor.Presentation.Framework.Tests
{
	using System.Threading;
	using MbUnit.Framework;
	using Binsor.Presentation.Framework.Layouts;
	using Binsor.Presentation.Framework.Tests.Demo;
	using System.Windows.Controls;

	[TestFixture(ApartmentState = ApartmentState.STA)]
	public class SingleViewPanelDecoratingLayoutFixture
	{
		[Test]
		public void When_add_a_view_to_decorating_layout_should_remove_all_other_children_in_the_panel()
		{
			Panel element = new DockPanel();
			element.Children.Add(new DockPanel());
			element.Children.Add(new DockPanel());

			Assert.AreEqual(2, element.Children.Count);

			PanelDecoratingLayout layout = new SingleViewPanelDecoratingLayout(element);
			layout.AddView(new DemoView());
			Assert.AreEqual(1, element.Children.Count);
		}
	}
}