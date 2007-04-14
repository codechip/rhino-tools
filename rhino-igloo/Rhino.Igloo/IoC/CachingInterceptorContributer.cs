using System;
using System.Collections.Generic;
using System.Data;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using NHibernate.Expression;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Inspect the model and add a caching interceptor if appropriate 
    /// </summary>
    public class CachingInterceptorContributer : IContributeComponentModelConstruction
    {
        /// <summary>
        /// Inspect the model and add a caching interceptor if appropriate 
        /// </summary>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            bool isRepository = model.Service.IsGenericType && 
                model.Service.GetGenericTypeDefinition() == typeof(IRepository<>);
            if (isRepository == false)
                return;
            Type entityType = model.Service.GetGenericArguments()[0];
            bool cacheable = entityType.GetCustomAttributes(typeof(CacheableAttribute), true).Length != 0;
            if(cacheable==false)
                return;
            model.Interceptors.Add(new InterceptorReference(typeof(CachingInterceptor)));
        }
    }
}