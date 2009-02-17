namespace Rhino.Commons.Test.Repository
{
    using System.IO;
    using MbUnit.Framework;
    using Rhino.Commons.ForTesting;

    [TestFixture]
    public class ActiveRecordRepositoryTests : RepositoryTests
    {
        [TestFixtureSetUp]
        public override void OneTimeTestInitialize()
        {
            base.OneTimeTestInitialize();
            string path =
                Path.GetFullPath(@"Repository\Windsor-AR.config");
            InitializeNHibernateAndIoC(PersistenceFramework.ActiveRecord, path, MappingInfoForRepositoryTests);
        }
    }
}