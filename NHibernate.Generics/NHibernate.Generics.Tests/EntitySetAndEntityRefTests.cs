using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate.Generics;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class EntitySetAndEntityRefTests
    {
        Blog blog;
        Post post;

        private EntitySet<Post> _posts;

        [SetUp]
        public void Setup()
        {
            blog = new Blog();
            post = new Post();
        }

        [Test]
        public void AddPostToBlogSetBlogInPost()
        {
            blog.Posts.Add(post);

            Assert.AreEqual(blog, post.Blog);
            Assert.AreEqual(1, blog.Posts.Count);
            Assert.IsTrue(blog.Posts.Contains(post));
        }

        [Test]
        public void DoubleAddDoesNothing()
        {
            blog.Posts.Add(post);
            blog.Posts.Add(post);

            Assert.AreEqual(blog, post.Blog);
            Assert.AreEqual(1, blog.Posts.Count);
            Assert.IsTrue(blog.Posts.Contains(post));
        }

        [Test]
        public void CallingClearCallItOnEachItemInTheSet()
        {
            Post post2 = new Post();
            blog.Posts.Add(post2);
            blog.Posts.Add(post);

            blog.Posts.Clear();

            Assert.IsNull(post2.Blog);
            Assert.IsNull(post.Blog);


        }


        [Test]
        public void AddPostToBlogAndRemoveResultInNullBlogInPost()
        {
            blog.Posts.Add(post);
            blog.Posts.Remove(post);

            Assert.IsNull(post.Blog);
            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
        }

        [Test]
        public void RemovingTwiceWorks()
        {
            blog.Posts.Add(post);
            blog.Posts.Remove(post);
            blog.Posts.Remove(post);

            Assert.IsNull(post.Blog);
            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
        }

        [Test]
        public void SetBlogAddToBlogsPosts()
        {
            post.Blog = blog;

            Assert.AreEqual(1, blog.Posts.Count);
            Assert.IsTrue(blog.Posts.Contains(post));
            Assert.AreEqual(blog, post.Blog);
        }

        [Test]
        public void ChangeCollectionWhileInsideAction()
        {
            //Require a class member variable, can't use a local
            //variable, because of delegates rules.
            _posts = new EntitySet<Post>(
            delegate(Post p)
            {
                using (_posts.AllowModifications)
                {
                    _posts.Add(new Post("Duplicate"));
                }
            }, null);
            _posts.Add(new Post("Original"));
            Assert.AreEqual(2, _posts.Count);

        }


        [Test]
        public void DoubleAssingmentDoesNothing()
        {
            post.Blog = blog;
            post.Blog = blog;

            Assert.AreEqual(1, blog.Posts.Count);
            Assert.IsTrue(blog.Posts.Contains(post));
            Assert.AreEqual(blog, post.Blog);
        }

        [Test]
        public void SetBlogToSecondBlogRemovesFromFirstBlogAndAddToSecond()
        {
            Blog secondBlog = new Blog();
            post.Blog = blog;
            post.Blog = secondBlog;

            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
            Assert.AreEqual(secondBlog, post.Blog);
            Assert.IsTrue(secondBlog.Posts.Contains(post));
        }

        [Test]
        public void SetBlogAndThenAddToSecondBlogRemovesFromFirstBlogAndAddToSecond()
        {
            Blog secondBlog = new Blog();
            post.Blog = blog;
            secondBlog.Posts.Add(post);

            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
            Assert.AreEqual(secondBlog, post.Blog);
            Assert.IsTrue(secondBlog.Posts.Contains(post));
        }

        [Test]
        public void AddToSecondBlogRemovedFromFirstBlogAndSetPost()
        {
            Blog secondBlog = new Blog();
            blog.Posts.Add(post);
            secondBlog.Posts.Add(post);


            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
            Assert.AreEqual(secondBlog, post.Blog);
            Assert.IsTrue(secondBlog.Posts.Contains(post));
        }

        [Test]
        public void SetToBlogAndThenAddToSecondBlogRemovedFromFirstBlogAndSetPost()
        {
            Blog secondBlog = new Blog();
            post.Blog = blog;
            secondBlog.Posts.Add(post);

            Assert.AreEqual(0, blog.Posts.Count);
            Assert.IsFalse(blog.Posts.Contains(post));
            Assert.AreEqual(secondBlog, post.Blog);
            Assert.IsTrue(secondBlog.Posts.Contains(post));
        }

        [Test]
        public void PassingNullForSetDelegates()
        {
            EntitySet<Post> posts = new EntitySet<Post>();
            posts.Add(post);
            Assert.AreEqual(1, posts.Count);
            posts.Remove(post);
            Assert.AreEqual(0, posts.Count);
        }

        [Test]
        public void PassingNullForRefDelegates()
        {
            EntityRef<Blog> blogRef = new EntityRef<Blog>();
            blogRef.Value = blog;
            Assert.AreSame(blog, blogRef.Value);
            blogRef.Value = null;
            Assert.IsNull(blogRef.Value);
        }


    }
}
