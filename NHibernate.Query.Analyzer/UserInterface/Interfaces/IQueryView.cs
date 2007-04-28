using System.Collections;
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Model;
using NHibernate;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IQueryView : IView
	{
		string QueryText { get; set; }
		bool ShowParams { get; set; }
		string HqlQueryText { get; }
		IDictionary Parameters { get; }
		string SqlQueryText { get; set; }
		void DisplayObjectGraph(IList list);
		void DisplayDataSet(DataSet ds);
		void SuggestParameter(string name);
		void RemoveSuggestParameter(string name);
		void RemoveParameter(string name);
		void SetParameterMissing(string name, bool missingState);
		void ShowObjectGraph();
		IQueryPresenter QueryPresenter { get; }
        

	}
}
