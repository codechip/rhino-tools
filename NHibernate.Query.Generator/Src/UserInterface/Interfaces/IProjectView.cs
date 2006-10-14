using Ayende.NHibernateQueryAnalyzer.Model;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IProjectView : IView
	{
		void DisplayProjectState(bool isEditable, bool allowUserEdit);
		IProjectPresenter ProjectPresenter { get; }
	}
}