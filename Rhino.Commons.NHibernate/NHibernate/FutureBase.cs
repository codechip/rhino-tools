namespace Rhino.Commons
{
	/// <summary>
	/// Base class for the future of a query, when you try to access the real
	/// result of the query than all the future queries in the current context
	/// (current thread / current request) are executed as a single batch and all
	/// their results are loaded in a single round trip.
	/// </summary>
	public class FutureBase
	{
		private const string cacheKey = "future.of.entity.batch.key";
		private bool wasLoaded;

		/// <summary>
		/// Gets a value indicating whether this instance was loaded.
		/// </summary>
		/// <value><c>true</c> if this query was loaded; otherwise, <c>false</c>.</value>
		protected bool WasLoaded
		{
			get { return wasLoaded; }
			set { wasLoaded = value; }
		}

		/// <summary>
		/// Gets the batcher.
		/// </summary>
		/// <value>The batcher.</value>
		protected static CriteriaBatch Batcher
		{
			get
			{
				var current = (CriteriaBatch) Local.Data[cacheKey];
				if (current == null)
					Local.Data[cacheKey] = current = new CriteriaBatch();
				return current;
			}
		}

		/// <summary>
		/// Execute all the queries in the batch.
		/// </summary>
		protected void ExecuteBatchQuery()
		{
			Batcher.Execute(UnitOfWork.CurrentSession);
			wasLoaded = true;
			ClearBatcher();
		}

		/// <summary>
		/// Clears the batcher.
		/// </summary>
		private static void ClearBatcher()
		{
			Local.Data[cacheKey] = null;
		}
	}
}