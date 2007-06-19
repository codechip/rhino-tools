using System;

namespace NHibernate.Query.Generator.Tests.WithEagerFetch
{
    /// <summary>
    /// The abstract base class describing actions that can be performed on an application.
    /// </summary>
    public abstract class Action : KeyedDomainObject<Guid>
    {
        private string _name;
        private string _description;
        private Application _application;
        private bool _obsolete;



        /// <summary>
        /// The name of action.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }



        /// <summary>
        /// A description for the action that may be set by the administrator.
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }



        /// <summary>
        /// The application to which the action applies.
        /// </summary>
        public virtual Application Application
        {
            get { return _application; }
            set
            {
                _application = value;
            }
        }



        /// <summary>
        /// Indicated that the action is not longer in use.
        /// </summary>
        public virtual bool Obsolete
        {
            get { return _obsolete; }
            set { _obsolete = value; }
        }
    }
}
