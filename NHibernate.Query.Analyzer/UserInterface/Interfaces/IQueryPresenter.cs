using System;
using Ayende.NHibernateQueryAnalyzer.Model;
using NHibernate;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IQueryPresenter : IPresenter
	{
		string TranslateHql();
		void ExecuteQuery();
		bool QueryCanBeTranslated();
		bool HasParameters(string queryText);
		bool AllParametersSet();
		void AnalyzeParameters();
		bool SaveQueryAs();
		bool SaveQuery();
		bool ReplaceException(Exception newException, Exception lastException);

		IQueryView View { get; }

		Query Query { get; }
	}
}