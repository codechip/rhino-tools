using System;
using HibernatingForums.Model;
using MbUnit.Framework;

namespace HibernatingForums.Tests
{
    [TestFixture]
    public class DiscussionThreadFixture
    {
        [Test]
        public void AddingPostToThreadModifyThreadLastUpdateDate()
        {
            DiscussionThread thread = new DiscussionThread();
            thread.LastUpdated = DateTime.Today.AddDays(-3);

            thread.AddPost(new Post());
            Assert.Between(thread.LastUpdated,
                                     DateTime.Now.AddSeconds(-10),
                                     DateTime.Now.AddSeconds(10)
                          );
        }

        [Test]
        public void AddPostToThreadAddToPostsCollection()
        {
            DiscussionThread thread = new DiscussionThread();
            Assert.AreEqual(0, thread.Posts.Count);
            thread.AddPost(new Post());
            Assert.AreEqual(1, thread.Posts.Count);
        }

        [Test]
        public void AddPostToThreadSetPostThreadToAddedThread()
        {
            DiscussionThread thread = new DiscussionThread();
            Post post = new Post();
            Assert.IsNull(post.Thread);
            thread.AddPost(post);
            Assert.AreEqual(thread, post.Thread);
           
        }
    }
}