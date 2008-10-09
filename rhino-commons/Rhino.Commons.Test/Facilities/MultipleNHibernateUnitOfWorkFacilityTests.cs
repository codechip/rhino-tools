using System;
using MbUnit.Framework;
using NHibernate;
using Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class MultipleNHibernateUnitOfWorkFacilityTests : MultipleNHibernateUnitOfWorkTestBase
    {
        [Test]
        public void Should_be_able_to_get_unit_of_work_by_name()
        {
            ISession s1 = UnitOfWork.GetCurrentSessionFor("database1");
            ISession s2 = UnitOfWork.GetCurrentSessionFor("database2");
            Assert.AreNotEqual(s1.SessionFactory, s2.SessionFactory);
        }

        [Test]
        public void Should_be_able_to_get_into_scope()
        {
            ISession s1 = UnitOfWork.GetCurrentSessionFor("database1");
            using(UnitOfWork.SetCurrentSessionName("database1"))
            {
                Assert.AreSame(UnitOfWork.CurrentSession, s1);
            }

            ISession s2= UnitOfWork.GetCurrentSessionFor("database2");
            using (UnitOfWork.SetCurrentSessionName("database2"))
            {
                Assert.AreSame(UnitOfWork.CurrentSession, s2);
            }
        }

        [Test]
        public void Should_be_able_to_get_domain_objects_from_multiple_databases()
        {
            Assert.IsNotNull(Repository<DomainObjectFromDatabase1>.Get(1));
            Assert.IsNotNull(Repository<DomainObjectFromDatabase2>.Get(1));
        }

        [Test]
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
                    AssertUnitsOfWorkAreEqual(current3, UnitOfWork.Current);
                }
                AssertUnitsOfWorkAreEqual(current2, UnitOfWork.Current);
            }
            AssertUnitsOfWorkAreEqual(current1, UnitOfWork.Current);
        }

        [Test]
        public void Should_be_able_to_nest_multi_unit_of_works_using_existing()
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

        private void AssertUnitsOfWorkAreEqual(IUnitOfWork expected, IUnitOfWork actual)
        {
            Assert.IsInstanceOfType(typeof(MultipleUnitsOfWorkImplementor), actual);
            Assert.IsInstanceOfType(typeof(MultipleUnitsOfWorkImplementor), expected);
            CollectionAssert.AreElementsEqual((MultipleUnitsOfWorkImplementor)expected, (MultipleUnitsOfWorkImplementor)actual);
        }
    }
}
