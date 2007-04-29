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
using System.Text;
using MbUnit.Framework;
using System.Collections;
using NHibernate.Generics.Tests.Properties;
using System.Reflection;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class EntityListTests
    {
        ISessionFactory factory;
        ISession session;

        [SetUp]
        public void SetUp()
        {
            factory = DatabaseTests.CreateSessionFactory();
        }

        [TearDown]
        public void CloseDispose()
        {
            if (session != null)
            {
                if (session.IsConnected)
                    session.Flush();
                session.Close();
                session.Dispose();
            }
        }

        [Test]
        public void CantAddDuplicateValue()
        {
            EntityList<string> strings = new EntityList<string>();
            strings.Add("ffo");
            strings.Add("ffo");
            strings.Add("ffo");

            Assert.AreEqual(1, strings.Count);
        }

        [Test]
        public void CanAddDuplicatesIfSetBehavior()
        {
            EntityList<string> strings = new EntityList<string>(OnDuplicate.Add);
            strings.Add("ffo");
            strings.Add("ffo");
            strings.Add("ffo");

            Assert.AreEqual(3, strings.Count);
        }

        #region Util
        public void CloseDisposeOpen()
        {
            CloseDispose();
            session = factory.OpenSession();
        }

        public int CreateAPost()
        {
            CloseDisposeOpen();
            Post p = new Post("test");
            p.Comments.Add(new Comment("dah"));
            session.Save(p);
            CloseDisposeOpen();
            return p.PostId;
        }
        #endregion

        /*
         * Simple tests (without NHibernate)
         */

        [Test]
        public void AddCommentToPostSetsPostOnComment()
        {
            Post p = new Post("Flaming");
            p.Comments.Add(new Comment("Ashes"));

            Assert.AreEqual(p, p.Comments[0].Post);

            Comment c = new Comment("backwards");
            c.Post = p = new Post("yeah");
            Assert.AreEqual(1, p.Comments.Count);
            Assert.AreEqual(p, p.Comments[0].Post);
        }

        [Test]
        public void CallingRemoveAtWillCallDelegate()
        {
            Post p = new Post("Flaming");
            Comment c = new Comment("Ashes");
            p.Comments.Add(c);

            p.Comments.RemoveAt(0);


            Assert.IsNull(c.Post);

        }
    

        [Test]
        public void DoubleAddOfSameInstanceDoesNothing()
        {
            /* I have modified EntityList.DoAdd with this to prevent adding the same item twice
             * 
            
            if (_list.Contains(item))
                return false;
            else
            {
                _list.Add(item);
                return true;
            }
             */
            Post p = new Post("1");
            Comment c;
            
            p.Comments.Add(new Comment("c1"));
            Assert.AreEqual(1, p.Comments.Count);

            p.Comments.Add(c = new Comment("c2"));
            Assert.AreEqual(2, p.Comments.Count);

            p.Comments.Add(c);
            Assert.AreEqual(2, p.Comments.Count, "Double test add failed");
        }

        [Test]
        public void CallCollectionClearSetsParentReferenceToNull()
        {
            Post p = new Post("Flaming");
            Comment c;
            p.Comments.Add(c = new Comment("Ashes"));
            p.Comments.Clear();
            Assert.AreEqual(0, p.Comments.Count);
            Assert.IsNull(c.Post);
        }

        [Test]
        public void ModifyCollectionsUsingInsertAtPositionAllowsDoubling()
        {
            Post p = new Post("q");
            Comment c = new Comment("a");

            p.Comments.Add(c);
            p.Comments.Insert(0,c);
            p.Comments.Add(c);

            Assert.AreEqual(2, p.Comments.Count);
        }

        /*
         * Tests with NHibernate
         */

        [Test]
        public void AddPostWithCommentToDatabase()
        {
            int id = CreateAPost();

            CloseDisposeOpen();
            Post pdb = (Post)session.Get(typeof(Post), id);
            Assert.IsNotNull(pdb);
            Assert.AreEqual(1, pdb.Comments.Count);
        }

        [Test]
        public void ClearCommentsCollection()
        {
            int id = CreateAPost();
            Post pdb;

            CloseDisposeOpen();
            pdb = (Post)session.Get(typeof(Post), id);
            pdb.Comments.Clear();

            CloseDisposeOpen();
            pdb = (Post)session.Get(typeof(Post), id);
            Assert.AreEqual(0, pdb.Comments.Count, "Not deleted on clear");
        }

        [Test]
        public void DeltePostCascadesToComments()
        {
            int id = CreateAPost();
            Post pdb;

            CloseDisposeOpen();
            pdb = (Post)session.Get(typeof(Post), id);
            session.Delete(pdb);

            CloseDisposeOpen();
            pdb = (Post)session.Get(typeof(Post), id);
            Assert.IsNull(pdb);
        }

        [Test]
        public void ExchangeCommentBetweenPostsDefault()
        {
            ExchangeCommentBetweenPosts(true);
        }


        [Test]
        public void CriteriaFetchJoin()
        {
            int id = CreateAPost();
            CloseDisposeOpen();

            Post pdb = (Post)session.CreateCriteria(typeof(Post))
                .Add(Expression.Expression.Eq("PostId", id))
                .SetFetchMode("Comments", FetchMode.Join)
                .UniqueResult();

            Assert.IsTrue(NHibernateUtil.IsInitialized(pdb.Comments));
        }

     
        private void ExchangeCommentBetweenPosts(bool showIgnoreForCascadeDeleteOrhpan)
        {
            int id1 = CreateAPost();
            int id2 = CreateAPost();

            Post pdb1, pdb2;
            Comment c1, c2;
            pdb1 = (Post)session.Get(typeof(Post), id1);
            pdb2 = (Post)session.Get(typeof(Post), id2);
            c1 = pdb1.Comments[0];
            c2 = pdb2.Comments[0];
            pdb1.Comments.Add(pdb2.Comments[0]);

            Assert.AreEqual(c1.Post, pdb1);
            Assert.AreEqual(c2.Post, pdb1);
            Assert.AreEqual(2, pdb1.Comments.Count);
            Assert.AreEqual(0, pdb2.Comments.Count);

            CloseDisposeOpen();
            pdb1 = (Post)session.Get(typeof(Post), id1);
            pdb2 = (Post)session.Get(typeof(Post), id2);

            if (showIgnoreForCascadeDeleteOrhpan)
            {
                if (pdb1.Comments.Count == 1)
                    Assert.Ignore("This test fails when delete cascade orhpan is set because the comment is deleted when removed from parent collection");

                Assert.AreEqual(2, pdb1.Comments.Count);
                Assert.AreEqual(0, pdb2.Comments.Count);
            }
            else
            {
                Assert.AreEqual(2, pdb1.Comments.Count);
                Assert.AreEqual(0, pdb2.Comments.Count);
            }
        }

        [Test]
        public void ExchangeCommentBetweenPostsWithCascadeAll()
        {
            Hashtable props = new Hashtable();
            props["hibernate.connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
            props["hibernate.dialect"] = "NHibernate.Dialect.MsSql2000Dialect";
            props["hibernate.connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            props["hibernate.connection.connection_string"] = Settings.Default.TestDatabase;
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            cfg.Properties = props;
            cfg.AddAssembly(Assembly.GetExecutingAssembly());

            cfg.GetClassMapping(typeof(Post)).GetProperty("Comments").Cascade = "all";
            factory = cfg.BuildSessionFactory();

            ExchangeCommentBetweenPosts(false);
        }
    }
}
