namespace Rhino.Security.Tests
{
    using System.Security.Principal;
    using Commons;
    using MbUnit.Framework;
    using Rhino.Commons.ForTesting;

    [TestFixture]
    public class ExternalUserIntegrationFixture : TestFixtureBase
    {
        protected Account account;
        protected IAuthorizationRepository authorizationRepository;
        protected IAuthorizationService authorizationService;
        protected IPermissionsBuilderService permissionsBuilderService;
        protected IPermissionsService permissionService;
        private WindowsIdentityUserAdapter user;

        [SetUp]
        public virtual void SetUp()
        {
            Security.PrepareForActiveRecordInitializationWithExternalUser<WindowsIdentityUserAdapter>(SecurityTableStructure.Prefix);
            MappingInfo from = MappingInfo.From(typeof (IUser).Assembly, typeof (User).Assembly);
            FixtureInitialize(PersistenceFramework.ActiveRecord, "windsor.boo", DatabaseEngine.MsSql2005, from);
            CurrentContext.CreateUnitOfWork();

            SetupEntities();
        }

        [Test]
        public void CanGetGroupsFromExternalUser()
        {
            UsersGroup[] groups = authorizationRepository.GetAssociatedUsersGroupFor(user);
            Assert.AreEqual(1, groups.Length);
            Assert.AreEqual("Administrators", groups[0].Name);
        }

        private void SetupEntities()
        {
            user = new WindowsIdentityUserAdapter(WindowsIdentity.GetCurrent());
         
            account = new Account();
            account.Name = "south sand";

            UnitOfWork.CurrentSession.Save(account);

            authorizationService = IoC.Resolve<IAuthorizationService>();
            permissionService = IoC.Resolve<IPermissionsService>();
            permissionsBuilderService = IoC.Resolve<IPermissionsBuilderService>();
            authorizationRepository = IoC.Resolve<IAuthorizationRepository>();
            authorizationRepository.CreateUsersGroup("Administrators");
            authorizationRepository.CreateEntitiesGroup("Important Accounts");
            authorizationRepository.CreateOperation("/Account/Edit");

            UnitOfWork.Current.TransactionalFlush();

            authorizationRepository.AssociateUserWith(user, "Administrators");
            authorizationRepository.AssociateEntityWith(account, "Important Accounts");

            UnitOfWork.Current.TransactionalFlush();
        }
    }
}