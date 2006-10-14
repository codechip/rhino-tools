using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Expression = NHibernate.Expression.Expression;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class UsingCascades
    {
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
        }

        [TearDown]
        public void TearDown()
        {
            session.Dispose();
        }

        [Test]
        public void UseCascades()
        {
            Blog blog = new Blog("Ayende @ Blog");
            for (int i = 0; i < 59; i++)
            {
                Post post = new Post(i.ToString());
                blog.Posts.Add(post);
            }

            session.Save(blog);

            session.Dispose();

            session = factory.OpenSession();

            Blog fromDb = (Blog)session.Load(typeof(Blog), blog.BlogID);
            Assert.AreEqual(59, fromDb.Posts.Count);
            
        }
    }
}
