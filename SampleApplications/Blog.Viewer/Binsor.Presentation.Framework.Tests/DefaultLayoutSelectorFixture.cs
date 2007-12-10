using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Binsor.Presentation.Framework.Layouts;

namespace Binsor.Presentation.Framework.Tests
{
	[TestFixture]
	public class DefaultLayoutSelectorFixture
	{
		[Test]
		public void RembmerToFixThis()
		{
			Assert.Fail("and add tests for auto get of selector from config as well");
		}
		/*
		[Test]
		public void When_asked_about_accepting_a_view_with_acceptable_name_should_return_true()
		{

			DefaultLayoutSelector selector = new DefaultLayoutSelector(layout);

			bool result = layout.CanAccept(new DemoView());
			Assert.IsTrue(result);
		}

		[Test]
		public void When_asked_about_accepting_a_view_with_an_uknown_name_should_return_false()
		{
			PanelDecoratingLayout layout = new PanelDecoratingLayout(null, new[]
			{
				"Barn",
				"Yard"
			});

			bool result = layout.CanAccept(new DemoView());
			Assert.IsFalse(result);
		}
		 * */
	}
}
