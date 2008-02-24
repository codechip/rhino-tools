namespace Rhino.Security
{
    using System;
    using System.Data;
    using NHibernate.SqlTypes;
    using NHibernate.UserTypes;

    /// <summary>
    /// Map an external user to a value that can be used in the 
    /// </summary>
    public class ExternalUserType<TExternalUser> : IUserType
        where TExternalUser : IExternalUser, new()
    {
        /// <summary>
        /// Compare two instances of the class mapped by this type for persistent "equality"
        /// ie. equality of persistent state
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        /// <summary>
        /// Get a hashcode for the instance, consistent with persistence "equality"
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        /// Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param>
        /// <param name="names">column names</param>
        /// <param name="owner">the containing entity</param>
        /// <returns></returns>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            object value = rs[names[0]];
            if( value == DBNull.Value)
                return null;
            TExternalUser user = new TExternalUser();
            user.Load(value.ToString());
            return user;
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param>
        /// <param name="value">the object to write</param>
        /// <param name="index">command parameter index</param>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            IDataParameter param = (IDataParameter) cmd.Parameters[index];
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = ((IUser) value).SecurityInfo.Identifier.ToString();
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">generally a collection element or entity field</param>
        /// <returns>a copy</returns>
        public object DeepCopy(object value)
        {
            return value;
        }

        /// <summary>
        /// During merge, replace the existing (<paramref name="target"/>) value in the entity
        /// we are merging to with a new (<paramref name="original"/>) value from the detached
        /// entity we are merging. For immutable objects, or null values, it is safe to simply
        /// return the first parameter. For mutable objects, it is safe to return a copy of the
        /// first parameter. For objects with component values, it might make sense to
        /// recursively replace component values.
        /// </summary>
        /// <param name="original">the value from the detached entity being merged</param>
        /// <param name="target">the value in the managed entity</param>
        /// <param name="owner">the managed entity</param>
        /// <returns>the value to be merged</returns>
        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param>
        /// <param name="owner">the owner of the cached object</param>
        /// <returns>
        /// a reconstructed object from the cachable representation
        /// </returns>
        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        /// <summary>
        /// Transform the object into its cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. That may not be enough
        /// for some implementations, however; for example, associations must be cached as
        /// identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param>
        /// <returns>a cacheable representation of the object</returns>
        public object Disassemble(object value)
        {
            return value;
        }

        /// <summary>
        /// The SQL types for the columns mapped by this type.
        /// </summary>
        /// <value></value>
        public SqlType[] SqlTypes
        {
            get { return new SqlType[] {new StringSqlType()}; }
        }

        /// <summary>
        /// The type returned by <c>NullSafeGet()</c>
        /// </summary>
        /// <value></value>
        public Type ReturnedType
        {
            get { return typeof (string); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        /// <value></value>
        public bool IsMutable
        {
            get { return false; }
        }
    }
}