using MbUnit.Framework;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
	[TestFixture]
	public class DefaultMockingStrategyTests: AutoMockingTests
	{

		[Test]
		public void DefaultMockingStrategyNeverNull()
		{
			Assert.IsNotNull(container.DefaultMockingStrategy,
				"Default mocking strategy property is null before actually trying to set null.");

			container.DefaultMockingStrategy = null;

			Assert.IsNotNull(container.DefaultMockingStrategy,
				"Default mocking strategy property is null after attempt to set null.");
		}

		[Test]
		public void DefaultMockingStrategyIsDynamicStrategy()
		{
			Assert.AreEqual(typeof(DynamicMockingStrategy), container.DefaultMockingStrategy.GetType(),
				"Default mocking strategy is not the Dynamic mocking strategy.");
		}

	}
}
