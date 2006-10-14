using Ayende.NHibernateQueryAnalyzer.Model;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IProjectPresenter
	{
		void BuildProject();
		void EditProject();
		
		bool SaveProjectAs();
		IProjectView View { get; }
		void ProjectViewDisposed();

		Project Project { get; }
		bool SaveProject();
	}
}