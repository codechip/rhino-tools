using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Binsor.Presentation.Framework.Layouts;
using Binsor.Presentation.Framework.Services;

namespace Binsor.Presentation.Framework.Tests
{
	using System.Threading;
	using System.Windows.Controls;
	using Binsor.Presentation.Framework.Tests.Demo;
	using System.Collections;

	[TestFixture(ApartmentState = ApartmentState.STA)]
	public class DefaultLayoutSelectorFixture
	{
		[Test]
		public void When_asked_about_accepting_a_view_with_same_name_and_no_override_defined_should_return_true()
		{
			DockPanel panel = new DockPanel { Name = "Demo" };
			PanelDecoratingLayout layout = new PanelDecoratingLayout(panel);
			DefaultLayoutSelector selector = new DefaultLayoutSelector(new Hashtable());

			bool result = selector.CanAccept(layout, new DemoView());
			Assert.IsTrue(result);
		}

		[Test]
		public void When_selector_has_configuration_for_layout_will_use_that_to_check_if_valid_view_for_layout()
		{

			DockPanel panel = new DockPanel { Name = "WithConfig" };
			PanelDecoratingLayout layout = new PanelDecoratingLayout(panel);
			var hash = new Hashtable();

			hash[layout.Name] = new[] { "WithConfig" };

			DefaultLayoutSelector selector = new DefaultLayoutSelector(hash);

			bool result = selector.CanAccept(layout, new DemoView());
			Assert.IsFalse(result);
		}

		[Test]
		public void When_asked_about_accepting_a_view_with_an_unknown_name_should_return_false()
		{
			DockPanel panel = new DockPanel { Name = "Uknnown" };
			PanelDecoratingLayout layout = new PanelDecoratingLayout(panel);
			DefaultLayoutSelector selector = new DefaultLayoutSelector(new Hashtable());

			bool result = selector.CanAccept(layout, new DemoView());
			Assert.IsFalse(result);
		}
	}
}
