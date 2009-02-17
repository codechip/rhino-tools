namespace Rhino.Commons.Test.HttpModules
{
	using System;
	using System.IO;
	using MbUnit.Framework;
	using Rhino.Commons.HttpModules;

	[TestFixture]
	public class UnitOfWorkApplicationFixture
	{
		[SetUp]
		public void TestInitialize()
		{
			IoC.Reset();
		}

		[Test]
		public void FirstRequestForUoWAppWillInitializeContainer()
		{
			UnitOfWorkApplication application = new UnitOfWorkApplication();
			application.Application_Start(this, EventArgs.Empty);
			Assert.IsNotNull(IoC.Container);
		}

		[Test]
		public void ReadingFileWillNotLockIt()
		{
			UnitOfWorkApplication application = new UnitOfWorkApplication();
			application.Application_Start(this, EventArgs.Empty);
			File.AppendAllText("windsor.boo", Environment.NewLine);
		}
	}
}
