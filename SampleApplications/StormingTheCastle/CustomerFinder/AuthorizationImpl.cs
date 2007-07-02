using System.Collections.Generic;
using NHibernate.Expression;

namespace CustomerFinder
{
    public class AuthorizationImpl : IAuthorization
    {
        private readonly IRepository repository;
        private readonly ILogger logger;

        public AuthorizationImpl(IRepository repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public bool IsAllowed(Customer customer, string operation)
        {
            DetachedCriteria criteria = DetachedCriteria.For<CustomerOperation>()
                .Add(Expression.Eq("Customer", customer))
                .CreateCriteria("Operation")
                .Add(Expression.Eq("Name", operation));
            ICollection<CustomerOperation> operations = repository.FindByCriteria<CustomerOperation>(criteria);
            if (operations.Count == 0)
            {
                logger.Log(string.Format("Permission denied: customer {0} for {1}", customer.Name, operation));
                return false;
            }
            else
            {
                logger.Log("Has permission on " + customer.Name + " to " + operation);
                return true;
            }
        }
    }
}