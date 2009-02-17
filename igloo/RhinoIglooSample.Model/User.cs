namespace RhinoIglooSample.Model
{
    using System;
    using Castle.ActiveRecord;

    [ActiveRecord]
    public class User
    {
        private int id;
        private string name;
        private string password;
        private DateTime passwordExpiresAt;

        [Property(NotNull = true)]
        public virtual DateTime PasswordExpiresAt
        {
            get { return passwordExpiresAt; }
            set { passwordExpiresAt = value; }
        }

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Property(NotNull = true)]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Property(NotNull = true)]
        public virtual string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
