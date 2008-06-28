using System.IO;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class ComponentForwardingTestCase : ConfigurationAsserts
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
			base.TestInitialize();
			string path = Path.GetFullPath(@"Binsor\ComponentForwarding.boo");
			_container = new RhinoContainer(path);
		}

		[Test]
		public void Can_forward_types()
		{
			Assert.AreSame(
				_container.Resolve<IRepository<Fubar>>(),
				_container.Resolve<IFubarRepository>()
			);
		}
	}
	public interface IFubarRepository : IRepository<Fubar>{}

	public class FubarRepository : FakeRepository<Fubar>, IFubarRepository
	{
		public FubarRepository()
			: base(null)
		{
		}
	}
}