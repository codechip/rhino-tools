using System;
using Rhino.Security.Configuration;
using Rhino.Security.Engine.Services;
using Rhino.Security.Framework;
using Rhino.Security.Framework.Builders;

namespace Rhino.Security.Tests
{
	using Commons;
	using MbUnit.Framework;
	using Rhino.Commons.ForTesting;

	public class DatabaseFixture : TestFixtureBase
	{
		protected Account account;
		protected IAuthorizationRepository authorizationRepository;
		protected IAuthorizationService authorizationService;
		protected IPermissionsBuilderService permissionsBuilderService;
		protected IPermissionsService permissionService;
		protected User user;

		[SetUp]
		public virtual void SetUp()
		{
			//Security.PrepareForActiveRecordInitialization<User>(SecurityTableStructure.Prefix);
			MappingInfo from = MappingInfo.From(typeof(AR.Permission).Assembly, typeof(User).Assembly);
			FixtureInitialize(PersistenceFramework.ActiveRecord, "windsorAR.boo", GetDatabaseEngine(), from);
			CurrentContext.CreateUnitOfWork();

			SetupEntities();
		}

		protected virtual DatabaseEngine GetDatabaseEngine()
		{
			return DatabaseEngine.MsSql2005;
		}

		[TearDown]
		public void TearDown()
		{
			CurrentContext.DisposeUnitOfWork();
		}

		private void SetupEntities()
		{
			user = new User();
			user.Name = "Ayende";
			account = new Account();
			account.Name = "south sand";

			UnitOfWork.CurrentSession.Save(user);
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