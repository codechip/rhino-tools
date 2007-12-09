namespace Binsor.Presentation.Framework.Tests
{
	using MbUnit.Framework;
	using Binsor.Presentation.Framework.Impl;
	using Binsor.Presentation.Framework.Tests.Demo;
	using Binsor.Presentation.Framework.Exceptions;
	using Rhino.Mocks;
	using Binsor.Presentation.Framework.Interfaces;

	[TestFixture]
	public class DefaultLayourRegistryFixture
	{
		[Test]
		public void After_a_layout_is_registered_if_can_be_retrieved_by_name()
		{
			DefaultLayoutRegistry registry = new DefaultLayoutRegistry();
			registry.RegisterLayout(new DemoLayout());

			Assert.IsNotNull(registry.GetLayout("Demo"));
		}

		[Test]
		[ExpectedException(typeof(DuplicateLayoutException),
			"Layout names must be unique. A layout named 'Demo' already exists.")]
		public void When_two_layouts_with_same_name_are_added_to_the_registry_an_exception_should_be_thrown()
		{
			DefaultLayoutRegistry registry = new DefaultLayoutRegistry();
			registry.RegisterLayout(new DemoLayout());
			registry.RegisterLayout(new DemoLayout());
		}

		[Test]
		public void When_view_added_to_registry_should_ask_layout_if_it_can_accept_view()
		{
			MockRepository mocks = new MockRepository();
			ILayout mockLayout = mocks.CreateMock<ILayout>();
			DemoView view = new DemoView();
			using (mocks.Record())
			{
				SetupResult.For(mockLayout.Name).Return("something");

				Expect.Call(mockLayout.CanAccept(view)).Return(false);
			}

			using (mocks.Record())
			{
				DefaultLayoutRegistry registry = new DefaultLayoutRegistry();
				registry.RegisterLayout(mockLayout);
				registry.AddView(view);
			}
		}

		[Test]
		public void When_layout_says_that_it_can_accept_view_registry_should_add_view_to_layout()
		{
			
		}
	}
}