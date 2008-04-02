using Rhino.Security.Framework;

namespace Rhino.Security.Tests
{
	using Commons;
	using MbUnit.Framework;

	[TestFixture]
	public class EnvironmentSetupFixture : DatabaseFixture
	{
		[Test]
		public void RepositoryIsNotProxied()
		{
			bool isProxy = IoC.Resolve<IRepository<IOperation>>()
				.GetType().FullName.Contains("Proxy");
			Assert.IsFalse(isProxy);
		}
	}
}