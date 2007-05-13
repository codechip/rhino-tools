using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang;
using Castle.ActiveRecord.Framework.Internal;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;

namespace Bumbler
{
	public partial class Bumble : IDisposable, IQuackFu
	{
		private static Dictionary<string, ISessionFactory> connectionStringToSessionFactoryCache = new Dictionary<string, ISessionFactory>();
		private readonly ISession session;

		public Bumble(ISession session)
		{
			this.session = session;
		}

		#region IDisposable Members

		public void Dispose()
		{
			session.Dispose();
		}

		#endregion

		#region IQuackFu Members

		public object QuackGet(string name, object[] parameters)
		{
			string entityName = Inflector.Singularize(name) ?? name;
			return Query("from " + entityName);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			if (name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase))
			{
				Type type = Type.GetType("Bumble." + name.Substring(3));
				if (type != null)
				{
					return session.Get(type, args[0]);
				}
			}
			throw new NotImplementedException("Can't call unknown method on bubmle");
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			throw new NotImplementedException("Can't set on bubmle!");
		}

		#endregion

		public IList Query(string query)
		{
			return session.CreateQuery(query).List();
		}

		public IList Criteria(DetachedCriteria criteria)
		{
			return criteria.GetExecutableCriteria(session).List();
		}

		public static Bumble Through(string connectionString)
		{
			if (connectionStringToSessionFactoryCache.ContainsKey(connectionString))
				return new Bumble(connectionStringToSessionFactoryCache[connectionString].OpenSession());
			ISchemaInspector inspector = new SqlServerSchemaInspector(connectionString);
			Assembly assembly = CodeGenHelper.GenerateAssemblyAndMappingFromSchema(inspector);
			//Note that here we are not thread safe, can't create two session factories at the same time.
			AppDomain.CurrentDomain.TypeResolve += delegate(object sender, ResolveEventArgs args)
			{
				if (args.Name.StartsWith("Bumble"))
					return assembly;
				return null;
			};
			ISessionFactory sessionFactory = BuildSessionFactory(assembly, connectionString);
			connectionStringToSessionFactoryCache[connectionString] = sessionFactory;
			return new Bumble(sessionFactory.OpenSession());
		}

		private static ISessionFactory BuildSessionFactory(Assembly assembly, string connectionString)
		{
			Configuration cfg = new Configuration();
			cfg.Properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			cfg.Properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");
			cfg.Properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			cfg.Properties.Add("hibernate.connection.connection_string", connectionString);
			cfg.AddAssembly(assembly);
			return cfg.BuildSessionFactory();
		}
	}
}