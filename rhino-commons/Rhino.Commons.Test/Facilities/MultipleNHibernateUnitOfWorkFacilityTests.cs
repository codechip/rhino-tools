using MbUnit.Framework;
using Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts;

namespace Rhino.Commons.Test.Facilities
{
	[TestFixture]
	public class MultipleNHibernateUnitOfWorkFacilityTests : MultipleNHibernateUnitOfWorkTestBase
	{
		[Test]
		public void Should_be_able_to_get_domain_objects_from_multiple_databases()
		{
			Assert.IsNotNull(Repository<DomainObjectFromDatabase1>.Get(1));
			Assert.IsNotNull(Repository<DomainObjectFromDatabase2>.Get(1));
		}

		[Test, Ignore]
		public void Should_be_able_to_nest_multi_unit_of_works_using_create_new()
		{
			IUnitOfWork current1 = UnitOfWork.Current;
			using (UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
			{
				IUnitOfWork current2 = UnitOfWork.Current;
				using (UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
				{
					IUnitOfWork current3 = UnitOfWork.Current;
					using (UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
					{

					}
					Assert.AreSame(current3, UnitOfWork.Current);
				}
				Assert.AreSame(current2, UnitOfWork.Current);
			}
			Assert.AreSame(current1, UnitOfWork.Current);
		}

		[Test]
		public void Should_be_able_to_nest_multi_unit_of_works_using_nesting()
		{
			IUnitOfWork current1 = UnitOfWork.Current;
			using (UnitOfWork.Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork))
			{
				IUnitOfWork current2 = UnitOfWork.Current;
				Assert.AreSame(current1, current2);
				using (UnitOfWork.Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork))
				{
					IUnitOfWork current3 = UnitOfWork.Current;
					Assert.AreSame(current2, current3);
					using (UnitOfWork.Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork))
					{

					}
					Assert.AreSame(current3, UnitOfWork.Current);
				}
				Assert.AreSame(current2, UnitOfWork.Current);
			}
			Assert.AreSame(current1, UnitOfWork.Current);
		}
	}
}
