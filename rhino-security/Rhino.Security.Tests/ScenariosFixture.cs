using Rhino.Commons.ForTesting;
using Rhino.Security.Framework;

namespace Rhino.Security.Tests
{
    using System;
    using Commons;
    using MbUnit.Framework;

  [TestFixture]
  public class ActiveRecord_ScenariosFixture
    : ScenariosFixture<User, AR.Operation>
  {
  }

  [TestFixture]
  public class NHibernate_ScenariosFixture
    : ScenariosFixture<User, NH.Operation>
  {
    protected override PersistenceFramework GetPersistenceFramework()
    {
      return PersistenceFramework.NHibernate;
    }
  }

    public abstract class ScenariosFixture<TUser, TOperation> : DatabaseFixture<TUser, TOperation>
      where TUser : class, IUser
      where TOperation : class, IOperation
    {
        [Test]
        public void DeeplyNestedUsersGroup()
        {
            IUsersGroup group = authorizationRepository.CreateUsersGroup("Root");
            UnitOfWork.Current.Flush();
            for (int j = 0; j < 50; j++)
            {
                group = authorizationRepository.CreateChildUserGroupOf(group.Name, "Child #" + j);
                UnitOfWork.Current.Flush();
            }
            UnitOfWork.Current.TransactionalFlush();

            authorizationRepository.AssociateUserWith(user, "Child #49");
            UnitOfWork.Current.TransactionalFlush();
            IUsersGroup[] groups = authorizationRepository.GetAncestryAssociation(user, "Root");
            Assert.AreEqual(51, groups.Length);
        }

        [Test]
        public void CanOnlyAssignAccountsThatAreAssignedToMe()
        {
            // during first deploy
            IOperation operation = authorizationRepository.CreateOperation("/Account/Assign");

            User secondUser = new User();
            secondUser.Name = "Second user";
            Repository<User>.Save(secondUser);

            // setup entities group for the users
            AddDefaultPermissions(operation, secondUser);
            AddDefaultPermissions(operation, user);

            UnitOfWork.Current.TransactionalFlush();

            authorizationRepository.AssociateEntityWith(account, "Assigned to " + user.Name);
            UnitOfWork.Current.TransactionalFlush();

            // validate that I can assign a case
            bool allowed = authorizationService.IsAllowed(user, account, "/Account/Assign");
            Assert.IsTrue(allowed);

            // validate that second cannot
            allowed = authorizationService.IsAllowed(secondUser, account, "/Account/Assign");
            Assert.IsFalse(allowed);

            // the act of assigning is simply moving from one entity group to another
            authorizationRepository.DetachEntityFromGroup(account, "Assigned to " + user.Name);
            authorizationRepository.AssociateEntityWith(account, "Assigned to " + secondUser.Name);

            // have to commit the transaction for it to work
            UnitOfWork.Current.TransactionalFlush();

            // validate that I can not longer assign a case
            allowed = authorizationService.IsAllowed(user, account, "/Account/Assign");
            Assert.IsFalse(allowed);

            // validate that second now can assign
            allowed = authorizationService.IsAllowed(secondUser, account, "/Account/Assign");
            Assert.IsTrue(allowed);
        }

        [Test]
        public void CanOnlyViewAccountsThatUserBelongsTo()
        {
            // on first deploy
            IOperation operation = authorizationRepository.CreateOperation("/Account/View");
            // when creating account
            IUsersGroup group = authorizationRepository.CreateUsersGroup("Belongs to " + account.Name);
            UnitOfWork.Current.TransactionalFlush();

            // setting permission so only associated users can view
            permissionsBuilderService
                .Allow(operation)
                .For(group)
                .On(account)
                .DefaultLevel()
                .Save();
            UnitOfWork.Current.TransactionalFlush();

            // when adding user to account
            authorizationRepository.AssociateUserWith(user, group);
            UnitOfWork.Current.TransactionalFlush();
            
            bool allowed = authorizationService.IsAllowed(user, account, "/Account/View");
            Assert.IsTrue(allowed);
        }

        private void AddDefaultPermissions(IOperation operation, User toUser)
        {
            IEntitiesGroup group = authorizationRepository.CreateEntitiesGroup("Assigned to " + toUser.Name);
            permissionsBuilderService
                .Allow(operation)
                .For(toUser)
                .On(group)
                .DefaultLevel()
                .Save();
        }
    }
}