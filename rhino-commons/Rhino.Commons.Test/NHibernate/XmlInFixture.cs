namespace Rhino.Commons.Test.NHibernate
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Commons.ForTesting;	
	using MbUnit.Framework;
	using global::NHibernate;
	using global::NHibernate.Criterion;

	[TestFixture]
	public class XmlInFixture : DatabaseTestFixtureBase
	{
		private ISession session;
		private Mail mailFromAdamToJane;
		private Mail mailFromBobToJane;
		private Person adam;
		private Person jane;
		private Person bob;

		public string NHibernateWindsorFilePath
		{
			get
			{
				return
					Path.GetFullPath(
						Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
									 @"ForTesting\Windsor-NH.config"));
			}
		}

		[TestFixtureSetUp]
		public void OneTimeTestInitialize()
		{
			InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, NHibernateWindsorFilePath, DatabaseEngine.MsSql2005,
									  "Test", MappingInfo.FromAssemblyContaining<Mail>());
		}


		[SetUp]
		public void TestInitialize()
		{
			jane = new Person();
			jane.Name = "jane";
			adam = new Person();
			adam.Name = "Adam";
			bob = new Person();
			bob.Name = "Bob";

			session = CurrentContext.CreateSession();

			mailFromAdamToJane = new Mail();
			mailFromAdamToJane.Subject = "Bella vez";
			mailFromAdamToJane.From = adam;
			mailFromAdamToJane.To = jane;
			session.Save(mailFromAdamToJane);

			mailFromBobToJane = new Mail();
			mailFromBobToJane.From = bob;
			mailFromBobToJane.To = jane;
			mailFromBobToJane.Subject = "Hello...";
			session.Save(mailFromBobToJane);
		}

		[Test]
		public void WithEntities()
		{
			System.Collections.IEnumerable persons = session.CreateCriteria(typeof (Person)).List<Person>();

			Assert.AreEqual(2,
			                session.CreateCriteria(typeof (Mail))
			                	.Add(XmlIn.Create("To", persons, 1))
								.List<Mail>()
			                	.Count);
		}

		[Test]
		public void WithValues()
		{
			int[] persons = new int[] {0, 1, 2};

			Assert.AreEqual(1,
			                session.CreateCriteria(typeof (Mail))
			                	.Add(XmlIn.Create("From.Id", persons, 1))
								.List<Mail>()
			                	.Count);
		}

		[Test]
		public void WithEntitiesAndNoResults()
		{			
			IList<Person> justBob = new List<Person>();
			justBob.Add(bob);

			Assert.AreEqual(0,
							session.CreateCriteria(typeof(Mail))
			                	.Add(XmlIn.Create("To", justBob))
			                	.List<Mail>()
			                	.Count);
		}

		[Test]
		public void TwoXmlIn()
		{
			System.Collections.IEnumerable persons = session.CreateCriteria(typeof (Person)).List<Person>();

			Assert.AreEqual(2,
							session.CreateCriteria(typeof(Mail))
			                	.Add(Restrictions.And(XmlIn.Create("From", persons, 1),
			                	                      XmlIn.Create("To", persons, 1)))
								.List<Mail>().Count);
		}

		[Test]
		public void XmlInNotUseXml()
		{
			System.Collections.IEnumerable persons = session.CreateCriteria(typeof(Person)).List<Person>();

			Assert.AreEqual(2,
							session.CreateCriteria(typeof(Mail))
								.Add(XmlIn.Create("To", persons))
								.List<Mail>()
								.Count);
		}
	}

	public class Mail
	{
		private int id;
		private string subject;
		private Person from;
		private Person to;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		public Person From
		{
			get { return from; }
			set { from = value; }
		}

		public Person To
		{
			get { return to; }
			set { to = value; }
		}
	}

	public class Person
	{
		private int id;
		private string name;
		private IList<Mail> mailSent = new List<Mail>();
		private IList<Mail> mailReceived = new List<Mail>();

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList<Mail> MailSent
		{
			get { return mailSent; }
			set { mailSent = value; }
		}

		public IList<Mail> MailReceived
		{
			get { return mailReceived; }
			set { mailReceived = value; }
		}
	}
}
