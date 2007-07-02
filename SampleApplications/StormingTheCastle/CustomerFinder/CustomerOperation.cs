using Castle.ActiveRecord;

namespace CustomerFinder
{
    [ActiveRecord]
    public class CustomerOperation
    {
        private Customer customer;
        private int id;
        private Operation operation;

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [BelongsTo]
        public virtual Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        [BelongsTo]
        public virtual Operation Operation
        {
            get { return operation; }
            set { operation = value; }
        }

    }
}