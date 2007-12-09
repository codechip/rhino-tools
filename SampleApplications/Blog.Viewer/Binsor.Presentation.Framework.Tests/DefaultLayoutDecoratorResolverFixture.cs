namespace Binsor.Presentation.Framework.Tests
{
	using System.Threading;
	using MbUnit.Framework;
	using Services;
	using System.Windows.Controls;
	using Rhino.Commons;
	using Binsor.Presentation.Framework.Interfaces;
	using Binsor.Presentation.Framework.Layouts;
	using Castle.Windsor;
	using Castle.MicroKernel;

	[TestFixture(ApartmentState = ApartmentState.STA)]
	public class DefaultLayoutDecoratorResolverFixture
	{
		private IWindsorContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new RhinoContainer("assembly://Binsor.Presentation.Framework.Tests/DefaultLayoutDecoratorResolverFixtureConfig.boo");
		}

		[Test]
		public void When_resolve_a_panel_should_return_panel_decorator_from_the_container()
		{
			DefaultLayoutDecoratorResolver resolvers = new DefaultLayoutDecoratorResolver(container.Kernel);

			DockPanel element = new DockPanel { Name = "MyElement" };
			var result = resolvers.GetLayoutDecoratorFor(element);
			Assert.IsInstanceOfType(typeof(PanelDecoratingLayout), result);
		}

		[Test]
		public void When_resolving_a_panel_layout_the_panel_instance_should_be_passed_to_the_layout()
		{
			DefaultLayoutDecoratorResolver resolvers = new DefaultLayoutDecoratorResolver(container.Kernel);

			DockPanel element = new DockPanel { Name = "MyElement" };
			var result = (PanelDecoratingLayout)resolvers.GetLayoutDecoratorFor(element);
			Assert.AreSame(element, result.Element);
		}

		[Test]
		public void When_cannot_find_configuration_for_element_should_return_null()
		{
			DefaultLayoutDecoratorResolver resolvers = new DefaultLayoutDecoratorResolver(new DefaultKernel());

			DockPanel element = new DockPanel { Name = "MyElement" };
			var result = (PanelDecoratingLayout)resolvers.GetLayoutDecoratorFor(element);
			Assert.IsNull(result);
		}
	}
}