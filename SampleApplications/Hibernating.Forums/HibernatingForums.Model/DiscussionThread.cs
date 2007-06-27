using System;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace HibernatingForums.Model
{
    [ActiveRecord]
    public class DiscussionThread
    {
        private int id;
        private string title;
        private DateTime lastUpdated = DateTime.Now;
        private Forum forum;
        private ISet<Post> posts = new HashedSet<Post>();

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Property(NotNull = true, Length = 255)]
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }

        [Property(NotNull = true)]
        public virtual DateTime LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }

        [BelongsTo(NotNull = true)]
        public virtual Forum Forum
        {
            get { return forum; }
            set { forum = value; }
        }

        [HasMany(Cascade = ManyRelationCascadeEnum.AllDeleteOrphan)]
        public virtual ISet<Post> Posts
        {
            get { return posts; }
            set { posts = value;}
        }

        public virtual void AddPost(Post post)
        {
            lastUpdated = DateTime.Now;
            posts.Add(post);
            post.Thread = this;
        }
    }
}