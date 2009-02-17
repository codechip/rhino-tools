namespace Rhino.Commons.Test.Repository
{
    using System.IO;
    using MbUnit.Framework;
    using Commons.ForTesting;

    [TestFixture]
    public class NHibernateRepositoryTests : RepositoryTests
    {
        [TestFixtureSetUp]
        public override void OneTimeTestInitialize()
        {
            base.OneTimeTestInitialize(); 
            string path = Path.GetFullPath(@"Repository\Windsor.config");
            InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, path, MappingInfoForRepositoryTests);
        }
    }
}