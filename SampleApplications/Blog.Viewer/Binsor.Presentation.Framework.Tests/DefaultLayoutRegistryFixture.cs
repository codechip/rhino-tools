namespace Binsor.Presentation.Framework.Tests
{
	using System.Threading;
	using MbUnit.Framework;
	using Binsor.Presentation.Framework.Services;
	using Binsor.Presentation.Framework.Tests.Demo;
	using Binsor.Presentation.Framework.Exceptions;
	using Rhino.Mocks;
	using Binsor.Presentation.Framework.Interfaces;
	using System.Windows.Controls;

	[TestFixture(ApartmentState =ApartmentState.STA)]
	public class DefaultLayoutRegistryFixture
	{
		[Test]
		public void After_a_layout_is_registered_if_can_be_retrieved_by_name()
		{
			DefaultLayoutRegistry registry = new DefaultLayoutRegistry(null, null);
			registry.Register(new DemoLayout());

			Assert.IsNotNull(registry.GetLayout("Demo"));
		}

		[Test]
		[ExpectedException(typeof(DuplicateLayoutException),
			"Layout names must be unique. A layout named 'Demo' already exists.")]
		public void When_two_layouts_with_same_name_are_added_to_the_registry_an_exception_should_be_thrown()
		{
			DefaultLayoutRegistry registry = new DefaultLayoutRegistry(null, null);
			registry.Register(new DemoLayout());
			registry.Register(new DemoLayout());
		}

		[Test]
		public void When_view_added_to_registry_should_ask_layout_if_it_can_accept_view()
		{
			MockRepository mocks = new MockRepository();
			ILayout stubbedLayout = mocks.Stub<ILayout>();
			ILayoutSelector mockSelector = mocks.CreateMock<ILayoutSelector>();
			DemoView view = new DemoView();
			using (mocks.Record())
			{
				SetupResult.For(stubbedLayout.Name).Return("something");

				Expect.Call(mockSelector.CanAccept(stubbedLayout, view)).Return(false);
			}

			using (mocks.Playback())
			{
				DefaultLayoutRegistry registry = new DefaultLayoutRegistry(null, mockSelector);
				registry.Register(stubbedLayout);
				registry.AddView(view);
			}
		}

		[Test]
		public void When_layout_says_that_it_can_accept_view_registry_should_add_view_to_layout()
		{
			MockRepository mocks = new MockRepository();
			ILayout mockLayout = mocks.CreateMock<ILayout>();
			ILayoutSelector mockSelector = mocks.Stub<ILayoutSelector>();
			DemoView view = new DemoView();
			using (mocks.Record())
			{
				SetupResult.For(mockLayout.Name).Return("something");

				Expect.Call(mockSelector.CanAccept(mockLayout, view)).Return(true);
				Expect.Call(delegate { mockLayout.AddView(view); });
			}

			using (mocks.Playback())
			{
				DefaultLayoutRegistry registry = new DefaultLayoutRegistry(null, mockSelector);
				registry.Register(mockLayout);
				registry.AddView(view);
			}
		}

		[Test]
		[ExpectedException(typeof(MissingLayoutException), "Could not find layout: Demo")]
		public void When_requesting_a_missing_layout_should_throw_meaningful_exception()
		{
			DefaultLayoutRegistry registry = new DefaultLayoutRegistry(null, null);
			registry.GetLayout("Demo");
		}

		[Test]
		public void When_registering_using_framework_element_will_add_layout_decorator_for_that_element()
		{
			var panel = new DockPanel { Name = "Demo" };

			MockRepository mocks = new MockRepository();
			ILayoutDecoratorResolver mockLayoutDecoratorResolver = mocks.CreateMock<ILayoutDecoratorResolver>();
			using (mocks.Record())
			{
				SetupResult.For(mockLayoutDecoratorResolver.GetLayoutDecoratorFor(panel))
					.Return(new DemoLayout());
			}

			using(mocks.Playback())
			{
				DefaultLayoutRegistry registry = new DefaultLayoutRegistry(mockLayoutDecoratorResolver, null);
				registry.Register(panel);
				Assert.IsNotNull(registry.GetLayout("Demo"));
			}
		}

		[Test]
		[ExpectedException(typeof(MissingLayoutException), "Could not find layout: Demo")]
		public void When_trying_to_register_framework_element_that_has_no_configured_layout_will_ignore_the_registration()
		{
			var panel = new DockPanel { Name = "Demo" };

			MockRepository mocks = new MockRepository();
			ILayoutDecoratorResolver mockLayoutDecoratorResolver = mocks.CreateMock<ILayoutDecoratorResolver>();
			using (mocks.Record())
			{
				SetupResult.For(mockLayoutDecoratorResolver.GetLayoutDecoratorFor(panel))
					.Return(null);
			}

			using (mocks.Playback())
			{
				DefaultLayoutRegistry registry = new DefaultLayoutRegistry(mockLayoutDecoratorResolver, null);
				registry.Register(panel);
				registry.GetLayout("Demo");
			}
		}
	}
}