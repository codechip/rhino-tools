using System.Collections;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Model;
using WeifenLuo.WinFormsUI;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IMainView : IView
	{
		IView ActiveDocument { get; }
		IEnumerable Documents { get; }
		IView[] ShowUnsavedDialog(IView[] unsavedView);
		Project SelectExistingProject();
		IMainPresenter MainPresenter { get; }
		Query SelectProjectQuery(Project prj);
		void Display(IView view);

		SaveFileDialog HbmSaveDlg { get; }

		SaveFileDialog CfgSaveDlg { get; }
	}
}