namespace Rhino.Commons
{
	public interface IUnitOfWorkImplementor : IUnitOfWork
	{
		void IncremementUsages();
		IUnitOfWorkImplementor Previous { get; }
	}
}