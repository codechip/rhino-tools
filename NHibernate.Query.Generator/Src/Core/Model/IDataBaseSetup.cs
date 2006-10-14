using NHibernate.Cfg;

namespace Ayende.NHibernateQueryAnalyzer.Model
{
	public interface IDataBaseSetup
	{
		void SetupDataBase(Configuration cfg);
	}
}