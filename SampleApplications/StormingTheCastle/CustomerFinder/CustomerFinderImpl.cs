using System.Collections.Generic;
using Castle.ActiveRecord;
using NHibernate.Expression;

namespace CustomerFinder
{
    public class CustomerFinderImpl : ICustomerFinder
    {
        private readonly IRepository repository;
        private readonly IAuditor auditor;
        private readonly IAuthorization authorization;

        public CustomerFinderImpl(IRepository repository, IAuditor auditor, IAuthorization authorization)
        {
            this.repository = repository;
            this.auditor = auditor;
            this.authorization = authorization;
        }

        public ICollection<Customer> FindCustomersByName(string name)
        {
            ICollection<Customer> customersFromRepository = 
                repository.FindByCriteria<Customer>(Expression.Eq("Name", name));
            List<Customer> approvedCustomers = new List<Customer>();
            foreach (Customer customer in customersFromRepository)
            {
                if(authorization.IsAllowed(customer, "View")==false)
                    continue;
                auditor.ReadCustomer(customer);
                approvedCustomers.Add(customer);
            }
            return approvedCustomers;
        }
    }
}