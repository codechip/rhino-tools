namespace Rhino.Commons
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using global::NHibernate.Criterion;

	/// <summary>
	/// Hold the future of a query, when you try to iterate over
	/// a instance of <see cref="FutureQueryOf{TEntity}"/> or access the Results
	/// or TotalCount properties, all the future queries in the current context
	/// (current thread / current request) are executed as a single batch and all
	/// their results are loaded in a single round trip.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class FutureQueryOf<TEntity> : IEnumerable<TEntity>
	{
		private const string cacheKey = "future.of.entity.batch.key";
		private ICollection<TEntity> results;
		private int? totalCount;

		public FutureQueryOf(DetachedCriteria detachedCriteria)
			: this(detachedCriteria, FutureQueryOptions.None)
		{
		}

		public FutureQueryOf(DetachedCriteria detachedCriteria, int firstResult, int maxResults)
			: this(detachedCriteria
			       	.SetFirstResult(firstResult)
			       	.SetMaxResults(maxResults),
			       FutureQueryOptions.WithTotalCount)
		{
		}

		public FutureQueryOf(DetachedCriteria detachedCriteria, FutureQueryOptions options)
		{
			CriteriaBatch criteriaBatch = Batcher.Add(detachedCriteria);

			switch (options)
			{
				case FutureQueryOptions.None:
					criteriaBatch.OnRead<TEntity>(delegate(ICollection<TEntity> entities)
					{
						results = entities;
					});
					break;
				case FutureQueryOptions.WithTotalCount:
					criteriaBatch.OnRead<TEntity>(delegate(ICollection<TEntity> entities, int count)
					{
						results = entities;
						totalCount = count;
					});
					break;
				default:
					throw new NotSupportedException(options.ToString());
			}
		}

		public int TotalCount
		{
			get
			{
				if (totalCount == null)
					LoadBatchQuery();
				if (totalCount == null)
					throw new InvalidOperationException("The future was not asked to load the total query as well, can't satisfy this requirement");
				return totalCount.Value;
			}
		}

		public ICollection<TEntity> Results
		{
			get
			{
				if (results == null)
					LoadBatchQuery();
				return results;
			}
		}

		private static CriteriaBatch Batcher
		{
			get
			{
				CriteriaBatch current = (CriteriaBatch) Local.Data[cacheKey];
				if (current == null)
					Local.Data[cacheKey] = current = new CriteriaBatch();
				return current;
			}
		}

		#region IEnumerable<TEntity> Members

		IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
		{
			return Results.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<TEntity>) this).GetEnumerator();
		}

		#endregion

		private static void LoadBatchQuery()
		{
			Batcher.Execute(UnitOfWork.CurrentSession);
			ClearBatcher();
		}

		private static void ClearBatcher()
		{
			Local.Data[cacheKey] = null;
		}
	}
}