using Castle.ActiveRecord;

namespace HibernatingForums.Model
{
    [ActiveRecord]
    public  class Post
    {
        private int id;
        private User author;
        private string title;
        private string content;
        private DiscussionThread thread;

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [BelongsTo(NotNull = true)]
        public virtual User Author
        {
            get { return author; }
            set { author = value; }
        }

        [Property(NotNull = true, Length = 255)]
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }

        [Property(NotNull = true)]
        public virtual string Content
        {
            get { return content; }
            set { content = value; }
        }

        [BelongsTo(NotNull = true)]
        public virtual DiscussionThread Thread
        {
            get { return thread; }
            set { thread = value; }
        }
    }
}