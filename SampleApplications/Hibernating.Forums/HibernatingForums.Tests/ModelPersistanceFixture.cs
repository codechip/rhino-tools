using System;
using System.Reflection;
using HibernatingForums.Model;
using MbUnit.Framework;
using Rhino.Commons;
using Rhino.Commons.ForTesting;

namespace HibernatingForums.Tests
{
    [TestFixture]
    public class ModelPersistanceFixture : ActiveRecordInMemoryTestFixtureBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            OneTimeInitalize("windsor.boo", Assembly.Load("HibernatingForums.Model"));
        }

        [SetUp]
        public void SetUp()
        {
            CreateUnitOfWork();
        }

        [TearDown]
        public void TearDown()
        {
            DisposeUnitOfWork();
        }

        [Test]
        public void CanSaveUser()
        {
            User user = CreateNewUser();

            Assert.AreEqual(0, user.Id);

            Repository<User>.Save(user);

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Evict(user);

            User fromDb = Repository<User>.Get(user.Id);
            Assert.AreNotSame(user, fromDb);

            Assert.AreEqual("Ayende", fromDb.Name);
            Assert.AreEqual("ayende@ayende.com", fromDb.Email);
            Assert.AreEqual(user.Id, fromDb.Id);
        }

        [Test]
        public void CanSaveForum()
        {
            User user = GetUser_Persistent();
            Forum forum = CreateForum(user);
            Repository<Forum>.Save(forum);

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Evict(forum);

            Forum fromDb = Repository<Forum>.Get(forum.Id);
            Assert.AreEqual("My Forum", fromDb.Name);
            Assert.AreEqual(forum.Manager.Id, fromDb.Manager.Id);
            Assert.AreEqual(forum.Id, fromDb.Id);
        }

        [Test]
        public void CanSaveDiscussionThread()
        {
            Forum forum = GetForum_Persistent();
            DiscussionThread dt = new DiscussionThread();
            dt.Title = "First Thread";
            dt.Forum = forum;
            Repository<DiscussionThread>.Save(dt);

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Evict(dt);

            DiscussionThread fromDb = Repository<DiscussionThread>.Get(dt.Id);
            Assert.AreNotSame(dt, fromDb);
            Assert.AreEqual("First Thread", fromDb.Title);
            Assert.Between(dt.LastUpdated,
                           DateTime.Now.AddSeconds(-10),
                           DateTime.Now.AddSeconds(10)
                );
            Assert.AreEqual(dt.Id, fromDb.Id);
        }



        [Test]
        public void CanSavePost()
        {
            Post post = new Post();
            post.Thread = GetThread_Persistent();

            post.Title = "First Post";
            post.Content = "Content";
            post.Author = post.Thread.Forum.Manager;

            Repository<Post>.Save(post);

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Evict(post);
            Post fromDb = Repository<Post>.Get(post.Id);
            Assert.AreNotSame(post, fromDb);

            Assert.AreEqual("First Post", fromDb.Title);
            Assert.AreEqual("Content", fromDb.Content);
            Assert.AreEqual(post.Author.Id, fromDb.Author.Id);
            Assert.AreEqual(post.Thread.Id, fromDb.Thread.Id);
        }

        [Test]
        public void ThreadPostsPersistenceByReachability()
        {
            Post post = new Post();
            DiscussionThread discussionThread = GetThread_Persistent();

            discussionThread.AddPost(post);

            post.Title = "First Post";
            post.Content = "Content";
            post.Author = post.Thread.Forum.Manager;

            UnitOfWork.Current.TransactionalFlush();
            UnitOfWork.CurrentSession.Evict(post);
            UnitOfWork.CurrentSession.Evict(discussionThread);

            Post fromDb = Repository<Post>.Get(post.Id);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(post, fromDb);

            DiscussionThread threadFromDb = fromDb.Thread;
            Assert.AreNotSame(discussionThread, threadFromDb);
            Assert.AreEqual(discussionThread.Id, threadFromDb.Id);

            Assert.IsTrue(threadFromDb.Posts.Contains(fromDb));
        }


        private User CreateNewUser()
        {
            User user = new User();
            user.Name = "Ayende";
            user.Email = "ayende@ayende.com";
            return user;
        }

        private Forum GetForum_Persistent()
        {
            User user = GetUser_Persistent();
            Forum forum = CreateForum(user);
            Repository<Forum>.Save(forum);
            return forum;
        }


        private User GetUser_Persistent()
        {
            User user = CreateNewUser();
            Repository<User>.Save(user);
            return user;
        }

        private Forum CreateForum(User user)
        {
            Forum forum = new Forum();
            forum.Name = "My Forum";
            forum.Manager = user;
            return forum;
        }

        private DiscussionThread GetThread_Persistent()
        {
            Forum forum = GetForum_Persistent();
            DiscussionThread dt = new DiscussionThread();
            dt.Title = "First Thread";
            dt.Forum = forum;
            Repository<DiscussionThread>.Save(dt);
            return dt;
        }

    }
}