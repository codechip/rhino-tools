using Castle.MicroKernel;
using MbUnit.Framework;
using Rhino.Commons.Facilities;

namespace Rhino.Commons.Test.Binsor
{
    [TestFixture]
    public class UnitOfWorkFacilityTestCase
    {
        [Test]
        public void CanCreateActiveRecordUnitOfWorkFacility()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UnitOfWorkFacilities.boo");
            IFacility[] facilities = container.Kernel.GetFacilities();
            ActiveRecordUnitOfWorkFacility activeRecordUnitOfWorkFactory = (ActiveRecordUnitOfWorkFacility)facilities[0];
            Assert.AreEqual(System.Reflection.Assembly.Load("Rhino.Commons.Binsor"),
                activeRecordUnitOfWorkFactory.Assemblies[0]);
        }

        [Test]
        public void ActiveRecordUnitOfWorkFacilityWillRegisterIRepositoryAndUnitOfWorkFactory()
        {
            RhinoContainer container = new RhinoContainer(@"Binsor\UnitOfWorkFacilities.boo");
            Assert.IsTrue(container.Kernel.HasComponent(typeof (IRepository<>)));
            Assert.IsTrue(container.Kernel.HasComponent(typeof(IUnitOfWorkFactory)));
        }
		[Test]
		public void ActiveRecordUnitOfWorkFacilityWillRegisterIRepositoryWithRepositoryKey()
		{
			RhinoContainer container = new RhinoContainer(@"Binsor\UnitOfWorkFacilities2.boo");
			Assert.IsTrue(container.Kernel.HasComponent("default_repository"));
		}
    }
}
