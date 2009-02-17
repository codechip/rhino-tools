namespace Rhino.Commons.Test.Repository
{
    using System.Collections.Generic;
    using System.IO;
    using MbUnit.Framework;
    using Rhino.Commons.ForTesting;

    [TestFixture]
    public class NHibernateRepositoryProjectionTests : RepositoryProjectionTests
    {
        [TestFixtureSetUp]
        public override void OneTimeTestInitialize()
        {
            base.OneTimeTestInitialize();
            string path =
                Path.GetFullPath(@"Repository\Windsor.config");

            InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, path, MappingInfoForRepositoryTests);
        }


        [Test]
        public void CanReportAllFromNamedQuery()
        {
            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>("QueryParentInfoByName",
                                                        new Parameter("nameOfPerson", "Parent%"));
            Assert.AreEqual(parentsInDb.Count, dtos.Count);
        }
    }
}