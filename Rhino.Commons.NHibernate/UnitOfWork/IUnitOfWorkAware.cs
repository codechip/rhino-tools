namespace Rhino.Commons
{
	/// <summary>
	/// Used to handle extra initialization for sessions such as filters, or to execute operations when a session is disposed.
	/// </summary>
	public interface IUnitOfWorkAware
	{
		/// <summary>
		/// Called when the UnitOfWork has been started, and UnitOfWork.Current is assigned
		/// </summary>
		/// <param name="unitOfWork">The newly started UnitOfWork</param>
		void UnitOfWorkStarted(IUnitOfWork unitOfWork);

		/// <summary>
		/// Called when the UnitOfWork is about to be disposed.
		/// </summary>
		/// <param name="unitOfWork">The UnitOfWork being disposed</param>
		void UnitOfWorkDisposing(IUnitOfWork unitOfWork);

		/// <summary>
		/// Called after the UnitOfWork has been disposed
		/// </summary>
		/// <param name="unitOfWork">The recently disposed UnitOfWork</param>
		void UnitOfWorkDisposed(IUnitOfWork unitOfWork);
	}
}