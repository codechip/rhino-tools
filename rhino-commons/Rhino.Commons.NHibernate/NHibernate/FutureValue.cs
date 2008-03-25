using NHibernate;
using NHibernate.Criterion;

namespace Rhino.Commons
{
	/// <summary>
	/// Base class for the future of a query, when you try to access the real
	/// result of the query than all the future queries in the current context
	/// (current thread / current request) are executed as a single batch and all
	/// their results are loaded in a single round trip.
	/// </summary>
	public class FutureValue<TEntity> : FutureBase
	{
		private readonly object id;
		private readonly FutureValueOptions options;
		private TEntity value;

		/// <summary>
		/// Initializes a new instance of the <see cref="FutureValue{TEntity}"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="options">The options.</param>
		public FutureValue(object id, FutureValueOptions options)
		{
			this.id = id;
			this.options = options;
			CriteriaBatch criteriaBatch = Batcher.Add(DetachedCriteria.For<TEntity>().Add(Restrictions.IdEq(id)));

			criteriaBatch.OnRead<TEntity>(delegate(TEntity entity) 
			{ 
				value = entity;
			    WasLoaded = true; 
			});
		}

		/// <summary>
		/// Gets the value, initializing the current batch if needed
		/// </summary>
		/// <value>The value.</value>
		public TEntity Value
		{
			get
			{
				if (WasLoaded == false)
					ExecuteBatchQuery();
				if (options == FutureValueOptions.ThrowIfNotFound && ReferenceEquals(value, null))
					throw new ObjectNotFoundException(id, typeof (TEntity).FullName);
				return value;
			}
		}
	}
}
