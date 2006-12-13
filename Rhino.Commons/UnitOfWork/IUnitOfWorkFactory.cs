namespace Rhino.Commons
{
	public interface IUnitOfWorkFactory
	{
		IUnitOfWorkImplementor Create(IUnitOfWorkImplementor previous);
	}
}