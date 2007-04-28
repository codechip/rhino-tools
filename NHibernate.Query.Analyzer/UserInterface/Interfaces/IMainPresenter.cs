using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IMainPresenter : IPresenter
	{
		IProjectView CurrentProjectView { get; set; }
		Project CurrentProject { get; set; }
		
		Project CreateNewProject();
		IProjectPresenter DisplayNewProject();

        IProjectPresenter OpenProject();
		bool CloseProject();
		bool CloseProjectChildren();
		void CloseCurrentDocument();

		void SaveDocument();
		void SaveDocumentAs();

		/// <summary>
		/// Executes the active query.
		/// This assume that the active windows is a QueryForm
		/// </summary>
		void ExecuteActiveQuery();
		void AddNewQuery();
		void OpenQuery();

		
		bool DeleteProject(Project project);
		void EnqueueCommand(ICommand command);

		IProjectsRepository Repository { get; }
		IQueue Queue { get; }
		IMainView View { get; }
		void CreateNewHbmDocument();
		void OpenHbmDocument(string name);

		void CreateNewCfgDocument();
		void OpenCfgDocument(string name);
        NHibernate.Cfg.Configuration NHibernateConfiguration { get; set; }
	}
}
