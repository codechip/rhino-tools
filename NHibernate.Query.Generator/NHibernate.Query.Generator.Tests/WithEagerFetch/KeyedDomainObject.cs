

namespace NHibernate.Query.Generator.Tests.WithEagerFetch
{
    /// <summary>
    /// Defines a domain object with an Id key (using a type defined by <typeparamref name="TId"/>) and
    /// a version for optimistic locking. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <typeparam name="TId">The type of the key. Typically an int, but can be a complex struct or class 
    /// (e.g. to have compound key).</typeparam>
    public abstract class KeyedDomainObject<TId>
    {
        private TId _id;
        private long _version;


        /// <summary>
        /// The key for the entity.
        /// </summary>
        public virtual TId Id
        {
            get { return _id; }
            protected set { _id = value; }
        }



        /// <summary>
        /// The version of the entity, used for optimistic locking strategies.
        /// </summary>
        public virtual long Version
        {
            get { return _version; }
            protected set { _version = value; }
        }



        /// <summary>
        /// Equality is tested against the key value for this instances.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            KeyedDomainObject<TId> other = obj as KeyedDomainObject<TId>;

            if(((object)other) == null)
            {
                return false;
            }

            return other.Id.Equals(this.Id);
        }



        /// <summary>
        /// The returned hashcode is based on the key value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }



        /// <summary>
        /// Compares KeyedDomainObjects for equality.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(KeyedDomainObject<TId> lhs, KeyedDomainObject<TId> rhs)
        {
            if (System.Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if ((object)lhs == null ^ (object)rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }



        /// <summary>
        /// Compares KeyedDomainObjects for inequality.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static bool operator !=(KeyedDomainObject<TId> lhs, KeyedDomainObject<TId> rhs)
        {
            return !(lhs == rhs);
        }
    }
}
