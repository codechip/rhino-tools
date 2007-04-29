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
