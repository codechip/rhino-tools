namespace RhinoIglooSample.Test.Model
{
    public class User : IUser
    {
        private int id;
        private string name = string.Empty;

        protected User()
        { 
        }

        public User(string name)
        {
            this.name = name;
        }

        public virtual int Id 
        { 
            get { return id; }
            set { id = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(User)) return false;

            User other = (User)obj;
            return other.id == id && other.name == name;
        }

        public override int GetHashCode()
        {
            return id + name.GetHashCode();
        }
    }
}
