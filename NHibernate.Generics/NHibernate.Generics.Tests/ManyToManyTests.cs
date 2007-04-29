#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NHibernate.Expression;
using NHibernate.Generics;

using MbUnit.Framework;

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
