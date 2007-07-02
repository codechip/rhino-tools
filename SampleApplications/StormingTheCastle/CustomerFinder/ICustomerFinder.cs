using System.Collections.Generic;

namespace CustomerFinder
{
    public interface ICustomerFinder
    {
        ICollection<Customer> FindCustomersByName(string name);
    }
}