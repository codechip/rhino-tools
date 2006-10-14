using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts
{
	public abstract class AssertBase : AbstractAsserter
	{
		public AssertBase(string message, params object[] args) : base(message, args)
		{
		}

		protected virtual void Fail()
		{
			AssertionFailureMessage message1 = new AssertionFailureMessage(this.message, this.args);
			throw new AssertionException(message1.ToString());
		}

		public abstract override void Assert();
	}
}