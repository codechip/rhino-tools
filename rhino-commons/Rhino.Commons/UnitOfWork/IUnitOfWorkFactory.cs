using System.Data;

namespace Rhino.Commons
{
	public interface IUnitOfWorkFactory
	{
		/// <summary>
		/// Initialize the factory, note that this may be called more than once
		/// </summary>
		void Init();

		/// <summary>
		/// Create a new unit of work implementation.
		/// </summary>
		/// <param name="maybeUserProvidedConnection">Possible connection that the user supplied</param>
		/// <param name="previous">Previous unit of work, if existed</param>
		/// <returns>A usable unit of work</returns>
		IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous);
	}
}