using System;
using Iesi.Collections.Generic;


namespace NHibernate.Query.Generator.Tests.WithEagerFetch
{
    /// <summary>
    /// Application represents a registed secure application.
    /// </summary>
    public class Application : KeyedDomainObject<Guid>
    {
        private string _name;
        private string _description;
        private bool _obsolete;
        private ISet<Operation> _operations = new HashedSet<Operation>();

        
        
        /// <summary>
        /// The name of role.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }



        /// <summary>
        /// A description for the role that may be set by the administrator.
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }



        /// <summary>
        /// All operations defined by this application.
        /// </summary>
        public virtual ISet<Operation> Operations
        {
            get { return _operations; }
            protected set { _operations = value; }
        }



        /// <summary>
        /// Indicated that the application is no longer in use.
        /// </summary>
        public virtual bool Obsolete
        {
            get { return _obsolete; }
            set { _obsolete = value; }
        }
    }
}
