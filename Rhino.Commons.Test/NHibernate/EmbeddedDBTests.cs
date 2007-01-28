using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NUnit.Framework;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.NHibernate
{
	[TestFixture]
	public class EmbeddedDBTests : NHibernateEmbeddedDBTestFixtureBase
	{
		private ISession session;

		[TestFixtureSetUp]
		public void OneTimeTestInitialize()
		{
			FixtureInitialize(typeof(SMS).Assembly);
		}

		[SetUp]
		public void TestInitialize()
		{
			session = this.CreateSession();
		}

		[TearDown]
		public void TestCleanup()
		{
			DisposeSession(session);
		}

		[Test]
		public void CanSaveAndLoadSMS()
		{
			SMS sms = new SMS();
			sms.Message = "R U There?";
			session.Save(sms);
			session.Flush();

			session.Evict(sms);//remove from session cache

			SMS loaded = session.Load<SMS>(sms.Id);
			Assert.AreEqual(sms.Message, loaded.Message);
		}
	}

	public class SMS
	{
		int id;
		string message;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}
