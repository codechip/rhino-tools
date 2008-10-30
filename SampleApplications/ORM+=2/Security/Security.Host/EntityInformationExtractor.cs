using System;
using HumanResources.Model;
using NHibernate.Criterion;
using Rhino.Commons;
using Rhino.Security.Interfaces;

namespace Security.Host
{
    public class EntityInformationExtractor<T> : IEntityInformationExtractor<T>
        where T : Entity
    {
        public Guid GetSecurityKeyFor(T entity)
        {
            return entity.SecurityKey;
        }

        public string GetDescription(Guid securityKey)
        {
            var result = 
                UnitOfWork.CurrentSession.CreateCriteria(typeof (T))
                    .Add(Restrictions.Eq("SecurityKey", securityKey))
                    .UniqueResult<T>();

            return result.ToString();
        }

        public string SecurityKeyPropertyName
        {
            get { return "SecurityKey"; }
        }
    }
}