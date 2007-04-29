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
