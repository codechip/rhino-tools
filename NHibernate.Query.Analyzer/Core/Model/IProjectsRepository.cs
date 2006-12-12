using System.Collections;
using NHibernate;

namespace Ayende.NHibernateQueryAnalyzer.Model
{
	public interface IProjectsRepository
	{
		/// <summary>
		/// Removes the project and all its associate data from NQA's database.
		/// </summary>
		/// <param name="project">The project to remove.</param>
		void RemoveProject(Project project);

		Project GetProjectById(int id);
		Project GetProjectByName(string projectName);

		IList GetProjectsStartingWith(string startProjectName);

		Project CreateProject(string projectName);
		void SaveProject(Project prj);
		void RemoveFromCache(Project current);
		IList GetAllProejcts();
		void SaveQuery(Query q);


	}
}