using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Generics.Tests
{
    public class Comment
    {
        private Comment()
        {
            _post = new EntityRef<Post>(
                delegate(Post p) { p.Comments.Add(this); },
                delegate(Post p) { p.Comments.Remove(this); }
                );
        }

        public Comment(string text):this()
        {
            _text = text;
        }

        int _id;
        int _indexInPost;
        string _text;
        EntityRef<Post> _post;

        public int IndexInPost
        {
            get { return _indexInPost; }
            set { _indexInPost = value; }
        }
        
        public Post Post
        {
            get { return _post.Value; }
            set { _post.Value = value; }
        }

        public int CommentId
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

    }
}
