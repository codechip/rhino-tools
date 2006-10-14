using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NHibernate.Expression;
using NHibernate.Generics;

using NUnit.Framework;

namespace NHibernate.Generics.Tests
{
	[TestFixture]
	public class ManyToManyTests
	{
		User ayende;
		User rahien;
		Blog tech;
		Blog fun;

		ISessionFactory factory;
		ISession session;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			factory = DatabaseTests.CreateSessionFactory();
		}

		[SetUp]
		public void Setup()
		{
			session = factory.OpenSession();

			ayende = new User("Ayende");
			rahien = new User("Rahien");
			tech = new Blog("Techincal");
			fun = new Blog("Funny Stuff");

			ayende.Blogs.Add(tech);
			ayende.Blogs.Add(fun);

			rahien.Blogs.Add(fun);
			rahien.Blogs.Add(tech);
		}

		[TearDown]
		public void TearDown()
		{
			session.Dispose();
		}

		[Test]
		public void AddBlogsToUsers()
		{
			Assert.AreEqual(2, ayende.Blogs.Count);
			Assert.AreEqual(2, rahien.Blogs.Count);

			Assert.AreEqual(2, tech.Users.Count);
			Assert.AreEqual(2, fun.Users.Count);
		}

		[Test]
		public void SaveToDatabase()
		{
			SaveDataAndReplaceSession();

			User ayendeFromDb = (User)session.Load(typeof(User), ayende.UserId);
			Assert.AreEqual(2, ayendeFromDb.Blogs.Count);
			foreach (Blog blog in ayendeFromDb.Blogs)
			{
				Assert.AreEqual(2, blog.Users.Count);
			}
		}

		private void SaveDataAndReplaceSession()
		{
			session.Save(ayende);
			session.Save(rahien);
			session.Flush();//This is important!
			session.Dispose();
			session = factory.OpenSession();
		}

		[Test]
		public void RemoveFromConnection()
		{
			SaveDataAndReplaceSession();

			User ayendeFromDb = (User)session.Load(typeof(User), ayende.UserId);
			ayendeFromDb.Blogs.Clear();

			session.Save(ayendeFromDb);
			session.Flush();
			session.Dispose();

			session = factory.OpenSession();

			Blog techFromDb = (Blog)session.Load(typeof(Blog), tech.BlogID);
			Assert.AreEqual(1, techFromDb.Users.Count);
		}

		[Test]
		public void LazyLoadingWhenTheSetsAreNotInitialized_AnInitOnLazySetToAlways()
		{
			SaveDataAndReplaceSession();
			User ayendeFromDb = (User)session.Load(typeof(User), ayende.UserId);

			EntitySet<Blog> blogs = ((EntitySet<Blog>)ayendeFromDb.Blogs);
			Assert.IsFalse(blogs.IsInitialized);


			ayendeFromDb.Blogs.Add(new Blog());

			Assert.IsTrue(blogs.IsInitialized);
			Assert.AreEqual(3, ayendeFromDb.Blogs.Count);

		}

		//Notice that here the famous edge-case appear,
		//since the blog side doesn't hold the 
		[Test]
		public void LazyLoadingWhenTheSetsAreNotInitialized()
		{
			SaveDataAndReplaceSession();
			Blog techFromDb = (Blog)session.Load(typeof(Blog), tech.BlogID);

			EntitySet<User> users = ((EntitySet<User>)techFromDb.Users);
			Assert.IsFalse(users.IsInitialized);


			techFromDb.Users.Add(new User());

			Assert.IsFalse(users.IsInitialized);
			Assert.AreEqual(2, techFromDb.Users.Count);

		}


	}
}
