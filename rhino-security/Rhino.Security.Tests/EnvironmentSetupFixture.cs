using Rhino.Commons.ForTesting;
using Rhino.Security.Framework;

namespace Rhino.Security.Tests
{
	using Commons;
	using MbUnit.Framework;

  [TestFixture]
  public class ActiveRecord_EnvironmentSetupFixture
    : EnvironmentSetupFixture<User, AR.Operation>
  {
  }

  [TestFixture]
  public class NHibernate_EnvironmentSetupFixture
    : EnvironmentSetupFixture<User, NH.Operation>
  {
    protected override PersistenceFramework GetPersistenceFramework()
    {
      return PersistenceFramework.NHibernate;
    }
  }

  public abstract class EnvironmentSetupFixture<TUser, TOperation> : DatabaseFixture<TUser, TOperation>
    where TUser : class, IUser
    where TOperation : class, IOperation
	{
		[Test]
		public void RepositoryIsNotProxied()
		{
			bool isProxy = IoC.Resolve<IRepository<TOperation>>()
				.GetType().FullName.Contains("Proxy");
			Assert.IsFalse(isProxy);
		}
	}
}