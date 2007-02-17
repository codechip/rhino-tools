using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class CheckingLazyLoading
    {

        Blog blog;
        Post post1, post2;
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
            blog = new Blog();
            post1 = new Post();
            post2 = new Post();
            post1.Blog = blog;
            post2.Blog = blog;

            session = factory.OpenSession();
            session.Save(blog);
            session.Save(post1);
            session.Save(post2);
            session.Dispose();

            session = factory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            session.Dispose();
        }

        [Test]
        public void LazyLoadingCollection()
        {
            Blog fromDb = (Blog )session.Load(typeof(Blog),blog.BlogID);
            Assert.IsFalse(
                NHibernateUtil.IsInitialized(((IWrapper)fromDb.Posts).Value)
            );
            int i = fromDb.Posts.Count;//init collection, don't ever do that on production!
            Assert.IsTrue(
                NHibernateUtil.IsInitialized(((IWrapper)fromDb.Posts).Value)
            );
        }

        [Test]
        public void LazyLoadingProperty()
        {
            Post fromDb = (Post)session.Load(typeof(Post), post2.PostId);
            Assert.IsFalse(
               NHibernateUtil.IsInitialized(fromDb.Blog)
           );
            string name = fromDb.Blog.BlogName;
            Assert.IsTrue(
               NHibernateUtil.IsInitialized(fromDb.Blog)
           );
        }
    
    
    }
}
