#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
