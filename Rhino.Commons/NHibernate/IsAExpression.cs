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

namespace Rhino.Commons
{
	using System;
	using System.Collections;
	using global::NHibernate;
	using global::NHibernate.Engine;
	using global::NHibernate.Expression;
	using global::NHibernate.Hql.Util;
	using global::NHibernate.Persister.Entity;
	using global::NHibernate.SqlCommand;
	using global::NHibernate.Type;

	public class IsAExpression : AbstractCriterion
	{
		private readonly Type _entityClass;
		private readonly string _propertyNameOrAlias;

		public IsAExpression(Type entityClass)
			: this(string.Empty, entityClass)
		{
		}

		public IsAExpression(string propertyNameOrAlias, Type entityClass)
		{
			_propertyNameOrAlias = propertyNameOrAlias;
			_entityClass = entityClass;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                                      IDictionary enabledFilters)
		{
			string alias;
			Type targetType;
			ICriteria aliasedCriteria;

			if (string.IsNullOrEmpty(_propertyNameOrAlias))
			{
				alias = criteriaQuery.GetSQLAlias(criteria);
				targetType = criteriaQuery.GetEntityName(criteria);
			}
			else if ((aliasedCriteria = criteria.GetCriteriaByAlias(_propertyNameOrAlias)) != null)
			{
				alias = criteriaQuery.GetSQLAlias(aliasedCriteria);
				targetType = criteriaQuery.GetEntityName(aliasedCriteria);		
			}
			else 
			{
				alias = criteriaQuery.GetSQLAlias(criteria, _propertyNameOrAlias);
				IType type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyNameOrAlias);

				if (!type.IsEntityType)
				{
					throw new QueryException("Only entities can be used with an IsAExpression");
				}

				targetType = type.ReturnedClass;
			}

			if (!targetType.IsAssignableFrom(_entityClass))
			{
				return new SqlString("1=0");	
			}

			IQueryable queryable = ObtainQueryable(criteriaQuery);
			SqlString condition = queryable.QueryWhereFragment(alias, true, true);

			if (condition.IndexOfCaseInsensitive(" and ") == 0)
			{
				condition = condition.Substring(5);
			}

			return (condition.Length > 0) ? condition : new SqlString("1=1");
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return EmptyTypes;
		}

		public override string ToString()
		{
			return _propertyNameOrAlias + " isa (" + _entityClass.FullName + ')';
		}

		public static IsAExpression Create(Type entityClass)
		{
			return new IsAExpression(entityClass);
		}

		public static IsAExpression Create<T>()
		{
			return new IsAExpression(typeof(T));
		}

		public static IsAExpression Create(string propertyNameOrAlias, Type entityClass)
		{
			return new IsAExpression(propertyNameOrAlias, entityClass);	
		}

		public static IsAExpression Create<T>(string propertyNameOrAlias)
		{
			return new IsAExpression(propertyNameOrAlias, typeof(T));
		}

		private IQueryable ObtainQueryable(ICriteriaQuery criteriaQuery)
		{
			IQueryable queryable = criteriaQuery.Factory.GetEntityPersister(_entityClass) as IQueryable;

			if (queryable == null)
			{
				queryable = SessionFactoryHelper.FindQueryableUsingImports(
				   criteriaQuery.Factory, _entityClass.FullName);
			}

			return queryable;
		}

		private static readonly TypedValue[] EmptyTypes = new TypedValue[0];
	}
}