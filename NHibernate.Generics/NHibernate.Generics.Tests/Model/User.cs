using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NHibernate.Generics.Tests
{
    public class User
    {
        string _userName;
        int _userId;
        EntitySet<Blog> _blogs;

        public virtual ICollection<Blog> Blogs
        {
            get { return _blogs; }
        }

		public virtual int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

		public virtual string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public User()
        {
            _blogs = new EntitySet<Blog>(
                delegate(Blog b) { b.Users.Add(this); },
                delegate(Blog b) { b.Users.Remove(this); },
                InitializeOnLazy.Always);
            
        }

        public User(string name)
            : this()
        {
            this._userName = name;
        }
    }
}
