namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface ICommand
	{
		void Execute();

		string Name { get; }

		string Info { get; }
	}
}