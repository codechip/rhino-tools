using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NHibernate.Generics.Tests
{
    public class Post
    {
        int post_id;
        EntityList<Comment> _comments;

        public virtual IList<Comment> Comments
        {
            get { return _comments; }
        }

		public virtual int PostId
        {
            get { return post_id; }
            set { post_id = value; }
        }
        string post_title;

		public virtual string PostTitle
        {
            get { return post_title; }
            set { post_title = value; }
        }

        EntityRef<Blog> _blog;

		public virtual Blog Blog
        {
            get { return _blog.Value; }
            set { _blog.Value = value; }
        }

        public Post()
        {
            _blog = new EntityRef<Blog>(
                delegate(Tests.Blog b) { b.Posts.Add(this); },
                delegate(Tests.Blog b) { b.Posts.Remove(this); }
                );
            _comments = new EntityList<Comment>(
                delegate(Comment c)
                {
                    c.Post = this;
                    c.IndexInPost = _comments.IndexOf(c);
                },
                delegate(Comment c)
                {
                    c.Post = null;
                    c.IndexInPost = -1;
                }
                );
        }

        public Post(string title)
            : this()
        {
            this.post_title = title;
        }
    }
}
