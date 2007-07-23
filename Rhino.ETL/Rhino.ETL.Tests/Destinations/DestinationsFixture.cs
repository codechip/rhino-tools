using MbUnit.Framework;

namespace Rhino.ETL.Tests.Destinations
{
	[TestFixture]
	public class DestinationsFixture
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = EtlContextBuilder.FromFile(@"Destinations\destinations_only.retl");
		}

		[Test]
		public void Destination_BatchSize_CanBeSpecifiedFromDSL()
		{
			DataDestination dest = configurationContext.Destinations["SouthSand"];
			Assert.AreEqual(500, dest.BatchSize );
		}
	}
}
