using Rhino.Security.Framework;

namespace Rhino.Security.Tests
{
    using System.Data;
    using Commons;
    using MbUnit.Framework;
    using Rhino.Commons.ForTesting;

  [TestFixture]
  public class ActiveRecord_AuthorizationServiceWithSecondLevelCacheFixture
    : AuthorizationServiceWithSecondLevelCacheFixture<User, AR.Operation>
  {
  }

  [TestFixture]
  public class NHibernate_AuthorizationServiceWithSecondLevelCacheFixture
    : AuthorizationServiceWithSecondLevelCacheFixture<User, NH.Operation>
  {
    protected override PersistenceFramework GetPersistenceFramework()
    {
      return PersistenceFramework.NHibernate;
    }
  }

  public abstract class AuthorizationServiceWithSecondLevelCacheFixture<TUser, TOperation> : DatabaseFixture<TUser, TOperation>
    where TUser : class, IUser
    where TOperation : class, IOperation
    {
        // we need those to ensure that we aren't leaving the 2nd level
        // cache in an inconsistent state after deletion
        //TODO: Add entity to group, save, remove and query
        //TODO: Add user to group, save, remove and query
        //TODO: Add nested users group save, remove and query

        [Test]
        public void UseSecondLevelCacheForSecurityQuestions()
        {
            permissionsBuilderService
                .Allow("/Account/Edit")
                .For(user)
                .On("Important Accounts")
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            using (UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
            {
                bool isAllowed = authorizationService.IsAllowed(user, account, "/Account/Edit");
                Assert.IsTrue(isAllowed);
            }

            using (IDbCommand command = UnitOfWork.CurrentSession.Connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM security_Permissions";
                command.ExecuteNonQuery();
            }
            using (UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
            {
                // should return true since it loads from cache
                bool isAllowed = authorizationService.IsAllowed(user, account, "/Account/Edit");
                Assert.IsTrue(isAllowed);
            }
        }


        /// <summary>
        /// We need this because we need to open several connections
        /// to the database.
        /// </summary>
        protected override DatabaseEngine GetDatabaseEngine()
        {
            return DatabaseEngine.MsSql2005;
        }
    }
}