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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Transform;

namespace Rhino.Commons
{
	public abstract class RepositoryImplBase<T>
	{
		private Type concreteType;

		private static readonly Order[] NullOrderArray = null;

		/// <summary>
		/// Gets or sets the concrete type of this repository
		/// </summary>
		/// <value>The type of the concrete.</value>
		public Type ConcreteType
		{
			get { return concreteType ?? typeof (T); }
			set { concreteType = value; }
		}

		protected abstract DisposableAction<ISession> ActionToBePerformedOnSessionUsedForDbFetches { get; }

		protected abstract ISessionFactory SessionFactory { get; }

		/// <summary>
		/// Get a future entity from the persistance store, or return null
		/// if it doesn't exist.
		/// Note that the null will be there when you resolve the FutureValue.Value property
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>A future for the value</returns>
		public FutureValue<T> FutureGet(object id)
		{
			return new FutureValue<T>(id, FutureValueOptions.NullIfNotFound);
		}

		/// <summary>
		/// A future of the entity loaded from the persistance store
		/// Will throw an exception if there isn't an entity that matches
		/// the id.
		/// </summary>
		/// <param name="id">The entity's id</param>
		/// <returns>The entity that matches the id</returns>
		public FutureValue<T> FutureLoad(object id)
		{
			return new FutureValue<T>(id, FutureValueOptions.ThrowIfNotFound);
		}


		/// <summary>
		/// Creates a <see cref="DetachedCriteria"/> compatible with this Repository
		/// </summary>
		/// <returns>The <see cref="DetachedCriteria"/></returns>
		public DetachedCriteria CreateDetachedCriteria()
		{
			return DetachedCriteria.For<T>();
		}

		/// <summary>
		/// Creates an aliases <see cref="DetachedCriteria"/> compatible with this Repository
		/// </summary>
		/// <param name="alias">the alias</param>
		/// <returns>The <see cref="DetachedCriteria"/></returns>
		public DetachedCriteria CreateDetachedCriteria(string alias)
		{
			return DetachedCriteria.For<T>(alias);
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(params ICriterion[] criteria)
		{
			return FindAll(NullOrderArray, criteria);
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="order">the order in which to bring the data</param>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order order, params ICriterion[] criteria)
		{
			return FindAll(new Order[] {order}, criteria);
		}

		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="orders">the order in which to bring the data</param>
		/// <param name="criteria">the criteria to look for</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(Order[] orders, params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria, orders);
				return crit.List<T>();
			}
		}


		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, 
					                                                       CreateCriteria, orders);
				return crit.List<T>();
			}
		}


		/// <summary>
		/// Loads all the entities that match the criteria
		/// by order
		/// </summary>
		/// <param name="criteria">the criteria to look for</param>
		/// <param name="orders"> the order to load the entities</param>
		/// <param name="firstResult">the first result to load</param>
		/// <param name="maxResults">the number of result to load</param>
		/// <returns>All the entities that match the criteria</returns>
		public ICollection<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, 
					                                                       CreateCriteria, orders);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(maxResults);
				return crit.List<T>();
			}
		}


		/// <summary>
		/// Loads all the entities that match the criteria, and allow paging.
		/// </summary>
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, params ICriterion[] criteria)
		{
			return FindAll(firstResult, numberOfResults, NullOrderArray, criteria);
		}


		/// <summary>
		/// Loads all the entities that match the criteria, with paging 
		/// and orderring by a single field.
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		/// <param name="selectionOrder">The field the repository should order by</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		/// </summary>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, Order selectionOrder, params ICriterion[] criteria)
		{
			return FindAll(firstResult, numberOfResults, new Order[] {selectionOrder}, criteria);
		}


		/// <summary>
		/// Loads all the entities that match the criteria, with paging 
		/// and orderring by a multiply fields.
		/// </summary>
		/// <param name="firstResult">The first result to load</param>
		/// <param name="numberOfResults">Total number of results to load</param>
		/// <param name="criteria">the cirteria to look for</param>
		/// <returns>number of Results of entities that match the criteria</returns>
		/// <param name="selectionOrder">The fields the repository should order by</param>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, Order[] selectionOrder,
		                              params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria, selectionOrder);
				crit.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults);
				return crit.List<T>();
			}
		}


		/// <summary>
		/// Execute the named query and return all the results
		/// </summary>
		/// <param name="namedQuery">The named query to execute</param>
		/// <param name="parameters">Parameters for the query</param>
		/// <returns>The results of the query</returns>
		public ICollection<T> FindAll(string namedQuery, params Parameter[] parameters)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(action.Value, namedQuery, parameters);
				return query.List<T>();
			}
		}


		/// <summary>
		/// Execute the named query and return paged results
		/// </summary>
		/// <param name="parameters">Parameters for the query</param>
		/// <param name="namedQuery">the query to execute</param>
		/// <param name="firstResult">The first result to return</param>
		/// <param name="numberOfResults">number of records to return</param>
		/// <returns>Paged results of the query</returns>
		public ICollection<T> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(action.Value, namedQuery, parameters);
				query.SetFirstResult(firstResult)
					.SetMaxResults(numberOfResults);
				return query.List<T>();
			}
		}


		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		public T FindOne(params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit =
					RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria);
				return crit.UniqueResult<T>();
			}
		}


		/// <summary>
		/// Find a single entity based on a criteria.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <returns>The entity or null</returns>
		public T FindOne(DetachedCriteria criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit =
					RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, CreateCriteria);
				return crit.UniqueResult<T>();
			}
		}


		/// <summary>
		/// Find a single entity based on a named query.
		/// Thorws is there is more than one result.
		/// </summary>
		/// <param name="parameters">parameters for the query</param>
		/// <param name="namedQuery">the query to executre</param>
		/// <returns>The entity or null</returns>
		public T FindOne(string namedQuery, params Parameter[] parameters)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(action.Value, namedQuery, parameters);
				return query.UniqueResult<T>();
			}
		}


		/// <summary>
		/// Find the first entity of type
		/// </summary>
		/// <param name="orders">Optional ordering</param>
		/// <returns>The entity or null</returns>
		public T FindFirst(params Order[] orders)
		{
			return FindFirst(null, orders);
		}


		/// <summary>
		/// Find the entity based on a criteria.
		/// </summary>
		/// <param name="criteria">The criteria to look for</param>
		/// <param name="orders">Optional orderring</param>
		/// <returns>The entity or null</returns>
		public T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, 
					                                                       CreateCriteria, orders);
				crit.SetFirstResult(0);
				crit.SetMaxResults(1);
				return (T) crit.UniqueResult();
			}
		}


		public ProjT ReportOne<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria,
					                                                       CreateCriteria);
				return DoReportOne<ProjT>(crit, projectionList);
			}
		}


		public ProjT ReportOne<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria);
				return DoReportOne<ProjT>(crit, projectionList);
			}
		}


		private static ProjT DoReportOne<ProjT>(ICriteria criteria, ProjectionList projectionList)
		{
			BuildProjectionCriteria<ProjT>(criteria, projectionList, false);
			return criteria.UniqueResult<ProjT>();
		}


		public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params Order[] orders)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null, 
					                                                       CreateCriteria, orders);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, bool distinctResults)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, null, CreateCriteria);
				return DoReportAll<ProjT>(crit, projectionList, distinctResults);
			}
		}


		public ICollection<ProjT> ReportAll_original<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, CreateCriteria);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, CreateCriteria);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(DetachedCriteria criteria, ProjectionList projectionList,
		                                           params Order[] orders)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, CreateCriteria, orders);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria, CreateCriteria);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjT> ReportAll<ProjT>(ProjectionList projectionList, Order[] orders, params ICriterion[] criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.CreateCriteriaFromArray(action.Value, criteria, CreateCriteria, orders);
				return DoReportAll<ProjT>(crit, projectionList);
			}
		}


		public ICollection<ProjJ> ReportAll<ProjJ>(string namedQuery, params Parameter[] parameters)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				IQuery query = RepositoryHelper<T>.CreateQuery(action.Value, namedQuery, parameters);
				return query.List<ProjJ>();
			}
		}


		/// <summary>
		/// Counts the number of instances matching the criteria.
		/// </summary>
		public long Count(DetachedCriteria criteria)
		{
			using (DisposableAction<ISession> action = ActionToBePerformedOnSessionUsedForDbFetches)
			{
				ICriteria crit = RepositoryHelper<T>.GetExecutableCriteria(action.Value, criteria, CreateCriteria);
				crit.SetProjection(Projections.RowCount());
				object countMayBe_Int32_Or_Int64_DependingOnDatabase = crit.UniqueResult();
				return Convert.ToInt64(countMayBe_Int32_Or_Int64_DependingOnDatabase);
			}
		}


		/// <summary>
		/// Counts the overall number of instances.
		/// </summary>
		public long Count()
		{
			return Count(null);
		}


		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists(DetachedCriteria criteria)
		{
			return 0 != Count(criteria);
		}


		/// <summary>
		/// Check if any instance of the type exists
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public bool Exists()
		{
			return Exists(null);
		}


		private static ICollection<ProjT> DoReportAll<ProjT>(ICriteria criteria, ProjectionList projectionList)
		{
			return DoReportAll<ProjT>(criteria, projectionList, false);
		}


		private static ICollection<ProjT> DoReportAll<ProjT>(ICriteria criteria, ProjectionList projectionList,
		                                                     bool distinctResults)
		{
			BuildProjectionCriteria<ProjT>(criteria, projectionList, distinctResults);
			return criteria.List<ProjT>();
		}


		private static void BuildProjectionCriteria<ProjT>(ICriteria criteria, IProjection projectionList,
		                                                   bool distinctResults)
		{
			if (distinctResults)
				criteria.SetProjection(Projections.Distinct(projectionList));
			else
				criteria.SetProjection(projectionList);

			if (typeof (ProjT) != typeof (object[]))
				//we are not returning a tuple, so we need the result transformer
			{
				criteria.SetResultTransformer(new TypedResultTransformer<ProjT>());
			}
		}


		public object ExecuteStoredProcedure(string sp_name, params Parameter[] parameters)
		{
			IConnectionProvider connectionProvider = ((ISessionFactoryImplementor)SessionFactory).ConnectionProvider;
			IDbConnection connection = connectionProvider.GetConnection();
			try
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sp_name;
					command.CommandType = CommandType.StoredProcedure;

					RepositoryHelper<T>.CreateDbDataParameters(command, parameters);

					return command.ExecuteScalar();
				}
			}
			finally
			{
				connectionProvider.CloseConnection(connection);
			}
		}


		/// <summary>
		/// Execute the specified stored procedure with the given parameters and then converts
		/// the results using the supplied delegate.
		/// </summary>
		/// <typeparam name="T2">The collection type to return.</typeparam>
		/// <param name="converter">The delegate which converts the raw results.</param>
		/// <param name="sp_name">The name of the stored procedure.</param>
		/// <param name="parameters">Parameters for the stored procedure.</param>
		/// <returns></returns>
		public ICollection<T2> ExecuteStoredProcedure<T2>(Converter<IDataReader, T2> converter, string sp_name,
		                                                  params Parameter[] parameters)
		{
			IConnectionProvider connectionProvider = ((ISessionFactoryImplementor)SessionFactory).ConnectionProvider;
			IDbConnection connection = connectionProvider.GetConnection();

			try
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = sp_name;
					command.CommandType = CommandType.StoredProcedure;

					RepositoryHelper<T>.CreateDbDataParameters(command, parameters);
					IDataReader reader = command.ExecuteReader();
					ICollection<T2> results = new List<T2>();

					while (reader.Read())
						results.Add(converter(reader));

					reader.Close();

					return results;
				}
			}
			finally
			{
				connectionProvider.CloseConnection(connection);
			}
		}

		/// <summary>
		/// Create an instance of <typeparamref name="T"/>, mapping it to the concrete class 
		/// if needed
		/// </summary>
		/// <returns></returns>
		public T Create()
		{
			return (T) Activator.CreateInstance(ConcreteType);
		}

		private ICriteria CreateCriteria(ISession session)
		{
			return CreateDetachedCriteria().GetExecutableCriteria(session);
		}

		#region Nested type: TypedResultTransformer

		/// <summary>
		/// This is used to convert the resulting tuples into strongly typed objects.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		private class TypedResultTransformer<T1> : IResultTransformer
		{
			#region IResultTransformer Members

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				if (tuple.Length == 1)
				{
					return tuple[0];
				}
				return Activator.CreateInstance(typeof (T1), tuple);
			}

			IList IResultTransformer.TransformList(IList collection)
			{
				return collection;
			}

			#endregion
		}

		#endregion
	}
}
