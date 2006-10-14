using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Property;
using System.Reflection;

namespace NHibernate.Generics
{
    /// <summary>
    /// This class is going to be used to get/set instnaces of EntitySet.
    /// There are some assumstions about the usage:
    ///  - Get from a property.
    ///  - Set via a field.
    /// <example>
    /// The usage of is expected to follow this pattern:
    /// public class Customer { ...
    /// 
    /// private EntitySet&lt;Order&gt; _orders;
    /// 
    /// public ICollection&lt;Order&gt; Orders
    /// {
    ///		get { return _orders; }
    /// }
    /// 
    /// public Customer()
    /// {
    ///		_orders = new EntitySet&ltOrder&gt;(
    ///			delegate(Order o) { o.Customer = this; },
    ///			delegate(Order o) { o.Customer = null; });
    ///		...
    /// }
    ///  
    /// ...
    /// 
    /// }
    /// </example>
    /// <remarks>
    /// The GetGetter and GetSetter are slightly modified version of the 
    /// ones that NHibernate uses.
    /// </remarks>
    /// </summary>
    public class GenericAccessor : IPropertyAccessor
    {
        protected IFieldNamingStrategy namingStrategy = new CamelCaseUnderscoreStrategy();

        #region Accessors
        public class CamelCaseUnderscore : GenericAccessor
        {
            public CamelCaseUnderscore()
            {
                base.namingStrategy = new CamelCaseUnderscoreStrategy();
            }
        }

        public class PascalCaseUnderscore : GenericAccessor
        {
            public PascalCaseUnderscore()
            {
                base.namingStrategy = new PascalCaseUnderscoreStrategy();
            }
        }

        public class PascalCaseMUnderscore : GenericAccessor
        {
            public PascalCaseMUnderscore()
            {
                base.namingStrategy = new PascalCaseMUnderscoreStrategy();
            }
        }

        public class CamelCase : GenericAccessor
        {
            public CamelCase()
            {
                base.namingStrategy = new CamelCaseStrategy();
            }
        }

        public class LowerCase : GenericAccessor
        {
            public LowerCase()
            {
                base.namingStrategy = new LowerCaseStrategy();
            }
        }

        public class LowerCaseUnderscore : GenericAccessor
        {
            public LowerCaseUnderscore()
            {
                base.namingStrategy = new LowerCaseUnderscoreStrategy();
            }
        } 
        #endregion

        #region IPropertyAccessor Members

        public IGetter GetGetter(System.Type theClass, string propertyName)
        {
            string fieldName = namingStrategy.GetFieldName(propertyName);
            FieldInfo field = GetField(theClass, fieldName);
            if (field == null) throw new PropertyNotFoundException(theClass, fieldName);
        	System.Type returnType = GetReturnType(field);
        	return new GenericGetter(field, theClass, fieldName, returnType); ;
        }

    	/// <summary>
    	/// Some heuristics to guess the return type
    	/// </summary>
    	private static System.Type GetReturnType(FieldInfo field)
    	{
    		foreach (System.Type type in field.FieldType.GetInterfaces())
    		{
    			if (type.IsGenericType &&
					typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
					return type.GetGenericArguments()[0];
    		}
			if (field.FieldType.IsGenericType &&
				typeof(EntityRef<>) == (field.FieldType.GetGenericTypeDefinition()))
				return field.FieldType.GetGenericArguments()[0];
			return field.FieldType;
    	}

    	public ISetter GetSetter(System.Type theClass, string propertyName)
        {
            string fieldName = namingStrategy.GetFieldName(propertyName);
            FieldInfo field = GetField(theClass, fieldName);
            if (field == null) throw new PropertyNotFoundException(theClass, fieldName);
			return new GenericSetter(field, theClass, fieldName);

        }

        #endregion

        /// <summary>
        /// Helper method to find the Field.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to find the Field in.</param>
        /// <param name="fieldName">The name of the Field to find.</param>
        /// <returns>
        /// The <see cref="FieldInfo"/> for the field.
        /// </returns>
        /// <exception cref="PropertyNotFoundException">
        /// Thrown when a field could not be found.
        /// </exception>
        internal static FieldInfo GetField(System.Type type, string fieldName)
        {
            if (type == null || type == typeof(object))
            {
                // the full inheritance chain has been walked and we could
                // not find the Field
                return null;
            }

            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field == null)
            {
                // recursively call this method for the base Type
                field = GetField(type.BaseType, fieldName);
            }

            return field;
        }
    }
}
