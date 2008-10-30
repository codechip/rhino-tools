using System.Collections.Generic;
using Interfaces;
using NHibernate.Criterion;
using NHibernate;

namespace CustomerCareModule
{
    public class CustomerCareService
    {
        private readonly ISession session;

        public CustomerCareService(ISession session)
        {
            this.session = session;
        }

        public void GenerateLeadFor(ICustomer customer, string lead)
        {
            session.Save(new Lead {Customer = customer, Note = lead});
        }

        public IEnumerable<Lead> GetLeadsFor(ICustomer customer)
        {
            return session.CreateCriteria(typeof (Lead))
                .Add(Restrictions.Eq("Customer", customer))
                .List<Lead>();
        }
    }
}