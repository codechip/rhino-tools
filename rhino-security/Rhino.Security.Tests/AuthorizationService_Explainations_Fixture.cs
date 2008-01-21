namespace Rhino.Security.Tests
{
    using Commons;
    using MbUnit.Framework;

    [TestFixture]
    public class AuthorizationService_Explainations_Fixture : DatabaseFixture
    {
        [Test]
        public void ExplainWhyAllowedWhenAllowedPermissionWasDefinedWithDenyPermissionWithLowerLevel()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .OnEverything()
                .Level(10)
                .Save();
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For("Administrators")
                .OnEverything()
                .Level(5)
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            string expected =
                @"Permission (level 10) for operation '/Account/Edit' was granted to 'Ayende' on 'everything'
Permission (level 5) for operation '/Account/Edit' was denied to group 'Administrators' on 'everything' ('Ayende' is a member of 'Administrators')
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyNotAllowedIfOperationNotDefined()
        {
            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Delete");
            Assert.AreEqual("Operation '/Account/Delete' was not defined\r\n", information.ToString());
        }

        [Test]
        public void ExplainWhyNotAllowedIfNoPermissionGranted()
        {
            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            Assert.AreEqual(
                "Permission for operation '/Account/Edit' was not granted to user 'Ayende' or to the groups 'Ayende' is associated with ('Administrators')\r\n",
                information.ToString());
        }

        [Test]
        public void ExplainWhyNotAllowedIfDenyPermissionWasDefined()
        {
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            Assert.AreEqual(
                "Permission (level 1) for operation '/Account/Edit' was denied to 'Ayende' on 'everything'\r\n",
                information.ToString());
        }

        [Test]
        public void ExplainWhyAllowedIfAllowPermissionWasDefined()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            Assert.AreEqual(
                "Permission (level 1) for operation '/Account/Edit' was granted to 'Ayende' on 'everything'\r\n",
                information.ToString());
        }

        [Test]
        public void ExplainWhyAllowedIfAllowPermissionWasDefinedOnGroup()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For("Administrators")
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            Assert.AreEqual(
                "Permission (level 1) for operation '/Account/Edit' was granted to group 'Administrators' on 'everything' ('Ayende' is a member of 'Administrators')\r\n",
                information.ToString());
        }


        [Test]
        public void ExplainWhyNotAllowedIfbDenyPermissionWasDefinedOnGroup()
        {
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For("Administrators")
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            Assert.AreEqual(
                "Permission (level 1) for operation '/Account/Edit' was denied to group 'Administrators' on 'everything' ('Ayende' is a member of 'Administrators')\r\n",
                information.ToString());
        }


        [Test]
        public void ExplainWhyNotAllowedIfAllowPermissionWasDefinedOnGroupAndDenyPermissionOnUser()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For("Administrators")
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was denied to group 'Administrators' on 'everything' ('Ayende' is a member of 'Administrators')
Permission (level 1) for operation '/Account/Edit' was granted to 'Ayende' on 'everything'
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyNotAllowedWhenAllowedPermissionWasDefinedWithDenyPermissionWithHigherLevel()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For("Administrators")
                .OnEverything()
                .Level(5)
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, "/Account/Edit");
            string expected =
                @"Permission (level 5) for operation '/Account/Edit' was denied to group 'Administrators' on 'everything' ('Ayende' is a member of 'Administrators')
Permission (level 1) for operation '/Account/Edit' was granted to 'Ayende' on 'everything'
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyAllowedOnAccountIfPermissionWasGrantedOnAnything()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was granted to 'Ayende' on 'everything'
";
            Assert.AreEqual(expected, information.ToString());
        }


        [Test]
        public void ExplainWhyDeniedOnAccountIfPermissionWasDeniedOnAnything()
        {
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For(user)
                .OnEverything()
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was denied to 'Ayende' on 'everything'
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyAllowedOnAccountIfPermissionWasGrantedOnGroupAssociatedWithUser()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For("Administrators")
                .On(account)
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was granted to group 'Administrators' on 'Account: south sand' ('Ayende' is a member of 'Administrators')
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyDeniedOnAccountIfPermissionWasDeniedOnGroupAssociatedWithUser()
        {
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For("Administrators")
                .On(account)
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was denied to group 'Administrators' on 'Account: south sand' ('Ayende' is a member of 'Administrators')
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test]
        public void ExplainWhyAllowedOnAccountIfPermissionWasGrantedToUser()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .On(account)
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was granted to 'Ayende' on 'Account: south sand'
";
            Assert.AreEqual(expected, information.ToString());
        }


        [Test]
        public void ExplainWhyDeniedOnAccountIfPermissionWasDeniedToUser()
        {
            permissionsBuilderService
                .Deny("/Account/Edit")
                .For(user)
                .On(account)
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =
                @"Permission (level 1) for operation '/Account/Edit' was denied to 'Ayende' on 'Account: south sand'
";
            Assert.AreEqual(expected, information.ToString());
        }

        [Test, Ignore("Need to think how we specify that the account is in the Important Accounts entities group")]
        public void ExplainWhyAllowedOnEntityGroupIfPermissionWasGrantedToUsersGroupAssociatedWithUser()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For("Administrators")
                .On("Important Accounts")
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();
            AuthorizationInformation information =
                authorizationService.GetAuthorizationInformation(user, account, "/Account/Edit");
            string expected =//what we currently have
                @"Permission (level 1) for operation '/Account/Edit' was granted to group 'Administrators' on 'Important Accounts' ('Ayende' is a member of 'Administrators')
";
            Assert.Fail("Account should be in important accounts");
            Assert.AreEqual(expected, information.ToString());
        }
    }
}