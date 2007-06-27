using System;
using Castle.ActiveRecord;

namespace HibernatingForums.Model
{
    [ActiveRecord]
    public class Forum
    {
        private int id;
        private string name;
        private User manager;

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        [BelongsTo(NotNull = true)]
        public virtual User Manager
        {
            get { return manager; }
            set { manager = value; }
        }
    }
}