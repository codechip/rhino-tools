using Castle.ActiveRecord.Framework;
using NHibernate;
using NHibernate.Cfg;

namespace Rhino.Commons
{
	/// <summary>
	/// Used to handle extra initialization for session factory
	/// </summary>
	public interface INHibernateInitializationAware
	{
		void Initialized(Configuration cfg, ISessionFactory sessionFactory);
	}
}
