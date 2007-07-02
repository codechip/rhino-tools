using Castle.ActiveRecord;

namespace CustomerFinder
{
    [ActiveRecord]
    public class Operation
    {
        private int id;
        private string name;

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
    }
}