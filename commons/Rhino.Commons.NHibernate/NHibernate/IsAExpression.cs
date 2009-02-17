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
	using System.Collections.Generic;
	using global::NHibernate;
	using global::NHibernate.Engine;
	using global::NHibernate.Criterion;
	using global::NHibernate.Hql.Util;
	using global::NHibernate.Persister.Entity;
	using global::NHibernate.SqlCommand;
	using global::NHibernate.Type;

	public class IsAExpression : AbstractCriterion
	{
		private readonly Type _entityClass;
		private readonly string _alias;

		public IsAExpression(Type entityClass)
			: this(string.Empty, entityClass)
		{
		}

		public IsAExpression(string alias, Type entityClass)
		{
			_alias = alias;
			_entityClass = entityClass;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
											  IDictionary<string, IFilter> enabledFilters)
		{
			string alias = null;
			Type targetType = null;
			ICriteria aliasedCriteria;

			if (string.IsNullOrEmpty(_alias))
			{
				alias = criteriaQuery.GetSQLAlias(criteria);
				string entityName = criteriaQuery.GetEntityName(criteria);
				targetType = criteriaQuery.Factory.GetEntityPersister(entityName).GetMappedClass(EntityMode.Poco);
			}
			else if ((aliasedCriteria = criteria.GetCriteriaByAlias(_alias)) != null)
			{
				alias = criteriaQuery.GetSQLAlias(aliasedCriteria);
				string entityName = criteriaQuery.GetEntityName(aliasedCriteria);
				targetType = criteriaQuery.Factory.GetEntityPersister(entityName).GetMappedClass(EntityMode.Poco);
			}

			if (targetType == null || !targetType.IsAssignableFrom(_entityClass))
			{
				return new SqlString("1=0");
			}

			IQueryable queryable = ObtainQueryable(criteriaQuery);
			string condition = queryable.FilterFragment(alias, enabledFilters);
			if (condition.IndexOf(" and ", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				condition = condition.Substring(5);
			}

			if (condition == string.Empty)
			{
				condition = "1=1";
			}

			return new SqlString(condition);
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return EmptyTypes;
		}

		public override string ToString()
		{
			return _alias + " isa (" + _entityClass.FullName + ')';
		}

		public static IsAExpression Create(Type entityClass)
		{
			return new IsAExpression(entityClass);
		}

		public static IsAExpression Create<T>()
		{
			return new IsAExpression(typeof(T));
		}

		public static IsAExpression Create(string alias, Type entityClass)
		{
			return new IsAExpression(alias, entityClass);
		}

		public static IsAExpression Create<T>(string alias)
		{
			return new IsAExpression(alias, typeof(T));
		}

		public override IProjection[] GetProjections()
		{
			return null;
		}

		private IQueryable ObtainQueryable(ICriteriaQuery criteriaQuery)
		{
			IQueryable queryable = criteriaQuery.Factory.GetEntityPersister(_entityClass.FullName) as IQueryable;

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
