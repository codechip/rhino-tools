using System.Collections.Generic;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace CustomerFinder
{
    [ActiveRecord]
    public class Customer
    {
        int id;
        string name;
        
        [PrimaryKey]
        public int Id
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