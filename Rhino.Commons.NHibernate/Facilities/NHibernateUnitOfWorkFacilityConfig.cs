using System;
using System.Collections.Generic;
using System.Reflection;
using Iesi.Collections.Generic;

namespace Rhino.Commons.Facilities
{
    public class NHibernateUnitOfWorkFacilityConfig
    {
        private ISet<Assembly> assemblies = new HashedSet<Assembly>();
        private ISet<Type> entities = new HashedSet<Type>();
        private string nhibernateConfigurationFile = "hibernate.cfg.xml";
        private bool registerEntities = false;

        public NHibernateUnitOfWorkFacilityConfig()
        { 
        }

        public NHibernateUnitOfWorkFacilityConfig(string assembly)
        {
            assemblies.Add(Assembly.Load(assembly));
        }

        public NHibernateUnitOfWorkFacilityConfig NHibernateConfiguration(string file)
        {
            nhibernateConfigurationFile = file;
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig RegisterEntities(bool register)
        {
            registerEntities = register;
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig AddAssembly(string assembly)
        {
            assemblies.Add(Assembly.Load(assembly));
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig AddEntity(Type entity)
        {
            entities.Add(entity);
            return this;
        }

        public bool ShouldRegisterEntitiesToRepository
        {
            get { return registerEntities; }
        }

        public Assembly[] Assemblies
        {
            get { return new List<Assembly>(assemblies).ToArray(); }
        }

        public Type[] Entities
        {
            get { return new List<Type>(entities).ToArray(); }
        }

        public string NHibernateConfigurationFile
        {
            get { return nhibernateConfigurationFile; }
        }
    }
}
