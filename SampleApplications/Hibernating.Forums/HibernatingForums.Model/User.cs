using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;

namespace HibernatingForums.Model
{
    [ActiveRecord]
    public class User
    {
        private int id;
        private string name;
        private string email;

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

        [Property(NotNull = true, Length = 50)]
        public virtual string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
