using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using NHibernate.Generics;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class AllowModifications
    {
        EntitySet<Post> posts;
        EntityRef<Blog> blog;

        [Test]
        public void AllowModifications_Set()
        {
            posts = new EntitySet<Post>(
                delegate(Post p)
                {
                    using (posts.AllowModifications)
                    {
                        posts.Add(new Post("Duplicate"));
                    }
                }, null);

            posts.Add(new Post("Original"));

            Assert.AreEqual(2, posts.Count);
        }

        [Test]
        public void AllowModifications_Ref()
        {
            Blog first = new Blog(), second = new Blog();
            blog = new EntityRef<Blog>(
                delegate(Blog b)
                {
                    using (blog.AllowModifications)
                    {
                        blog.Value = second;
                    }
                }, null);
            blog.Value = first;

            Assert.AreSame(second, blog.Value);
        }
    }
}
