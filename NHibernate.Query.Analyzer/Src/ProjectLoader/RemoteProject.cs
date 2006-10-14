using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Type;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
	public class RemoteProject : MarshalByRefObject, IDisposable
	{
		#region Variables

		private ILog logger = LogManager.GetLogger(typeof (RemoteProject));

		private Configuration cfg;
		private ISessionFactory factory;
		private Settings settings;
		private Hashtable loadedAssemblies = new Hashtable();
		private HqlResultGraph currentHqlGraph;
		private IList assemblies;
		private IList basePaths;
        private static readonly MethodInfo prepareQueryCommand = typeof(QueryTranslator).GetMethod("PrepareQueryCommand", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo populateSqlString = typeof(QueryTranslator).GetMethod("PopulateSqlString", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo getQuery = Type.GetType("NHibernate.Impl.SessionFactoryImpl,NHibernate").GetMethod("GetQuery");

	    #endregion

		#region Properties

		public Configuration Cfg
		{
			get { return cfg; }
		}

		public ISessionFactory Factory
		{
			get { return factory; }
		}

		public Settings Settings
		{
			get { return settings; }
		}

		public Hashtable LoadedAssemblies
		{
			get { return loadedAssemblies; }
		}

		#endregion

		#region Constructors

		public RemoteProject()
		{
			cfg = new Configuration();
		}

		#endregion

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public string[] HqlToSql(string hqlQuery, IDictionary parameters)
		{
			using (ISessionImplementor session = (ISessionImplementor) factory.OpenSession())
			{
				IList list = HqlToSqlList(hqlQuery,
				                          CreateQueryParameters(parameters), session);
				ArrayList cmds = new ArrayList();
				foreach (string sqlString in list)
				{
					cmds.Add(sqlString);
					if (logger.IsDebugEnabled)
						logger.Debug("Resulting SQL: " + sqlString);
				}
				return (string[]) cmds.ToArray(typeof (string));
			}
		}

		private QueryParameters CreateQueryParameters(IDictionary parameters)
		{
			QueryParameters qp = new QueryParameters(new IType[0], new object[0]);
			Hashtable ht = new Hashtable(parameters.Count);
			foreach (DictionaryEntry parameter in parameters)
			{
				TypedParameter tp = (TypedParameter) parameter.Value;
				ht[parameter.Key] = new TypedValue(tp.IType, tp.Value);
			}
			qp.NamedParameters = ht;
			return qp;
		}

		internal IList HqlToSqlList(string hqlQuery, QueryParameters qp, ISessionImplementor session)
		{
			if (logger.IsDebugEnabled)
				logger.Debug("Translating HQL Query: " + hqlQuery);
            IList commands = HqlToCommandList(hqlQuery, qp, session);
            IList sql = new ArrayList();
            foreach (IDbCommand command in commands)
		    {
                sql.Add(command.CommandText);
		    }
			return sql;
		}

        private IList HqlToCommandList(string hqlQuery, QueryParameters qp, ISessionImplementor session)
        {
            ISessionFactoryImplementor factoryImplementor = (ISessionFactoryImplementor)factory;
            PropertyInfo sqlString = typeof(QueryTranslator).GetProperty("SqlString", BindingFlags.NonPublic | BindingFlags.Instance);
            QueryTranslator[] queryTranslators = (QueryTranslator[])getQuery.Invoke(factoryImplementor, new object[] { hqlQuery, false });
            IList commands = new ArrayList();
            foreach (QueryTranslator translator in queryTranslators)
            {
                populateSqlString.Invoke(translator, new object[] { qp });
                IDbCommand cmd = (IDbCommand)prepareQueryCommand.Invoke(translator,
                    new object[] { sqlString.GetValue(translator, null), qp, false, session });
                commands.Add(cmd);
            }
            return commands;
        }

		private static void AddParameters(TypedParameter[] parameters, IQuery query)
		{
			foreach (TypedParameter parameter in parameters)
			{
				query.SetParameter(parameter.Name, parameter.Value,
				                   parameter.IType);
			}
		}

		#region Load Project

		//TODO: What happen if an exception is thrown across app-domain boundry?
		public void BuildInternalProject(IList assemblies, IList mappings, IList configurations, IList basePaths)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			this.basePaths = basePaths;
			this.assemblies = assemblies;
            basePaths.Add(AppDomain.CurrentDomain.BaseDirectory);		    
			LoadAssemblies(assemblies);
			LoadConfigurations(configurations);
			LoadMappings(mappings);
			factory = cfg.BuildSessionFactory();
			settings = SettingsFactory.BuildSettings(cfg.Properties);
		}

		private void LoadAssemblies(IList assembliesToLoad)
		{
			foreach (string assemblyFileName in assembliesToLoad)
			{
				if (logger.IsDebugEnabled)
					logger.Debug("Loading assembly: " + assemblyFileName);
				Assembly assembly = LoadAssembly(assemblyFileName);
                if (!basePaths.Contains(assemblyFileName))
                    basePaths.Add(Path.GetDirectoryName(assemblyFileName));
				cfg.AddAssembly(assembly);
			}
		}

		private void LoadMappings(IList mappings)
		{
			foreach (string mapping in mappings)
			{
				if (logger.IsDebugEnabled)
					logger.Debug("Loading mapping: " + mapping);
				cfg.AddXmlFile(mapping);
			}
		}

		private void LoadConfigurations(IList configurations)
		{
			foreach (string configuration in configurations)
			{
				if (logger.IsDebugEnabled)
					logger.Debug("Loading configurations: " + configuration);
				cfg.Configure(configuration);
			}
		}

		#endregion

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return LoadAssembly(args.Name);
		}

		private Assembly LoadAssembly(string assemblyName)
		{
			if (logger.IsInfoEnabled)
				logger.Info("Resolving assembly: " + assemblyName);
			Assembly asm = (Assembly) loadedAssemblies[assemblyName];
			if (asm == null)
			{
				if (logger.IsInfoEnabled)
					logger.Info("Loading assembly: " + assemblyName);
				try
				{
					if (File.Exists(assemblyName))
					{
						asm = Assembly.LoadFile(GetAssemblyFilename(assemblyName));
						if (logger.IsInfoEnabled)
							logger.Info("Successfully loaded: " + asm.FullName);
					}
					else
					{
						if (logger.IsInfoEnabled)
							logger.Info("Trying to load assembly: " + assemblyName);
						string asmPath = TryGuessAssemblyFilename(assemblyName);
						asm = Assembly.LoadFrom(asmPath);
					}
					
				}
				catch (Exception ex)
				{
					if (logger.IsErrorEnabled)
						logger.Error(ex);
					throw;
                }
			    //register loaded assembly
                loadedAssemblies[assemblyName] = asm; //filename
                loadedAssemblies[asm.FullName] = asm; //full assembly name
                loadedAssemblies[asm.GetName().Name] = asm; //short assembly name
			}
			return asm;
		}

		private string TryGuessAssemblyFilename(string name)
		{
			name = name.Split(',')[0]; //Get just the assembly name
          	foreach (string path in basePaths)
			{
				string filename = Path.Combine(path, name);
				string dll = filename + ".dll", exe = filename + ".exe";
				if (File.Exists(dll))
					return dll;
				if (File.Exists(exe))
					return exe;
			}
			return name;
		}

		public HqlResultGraph RunHql(string hql, params TypedParameter[] parameters)
		{
			try
			{
				ISession session = factory.OpenSession();
				IQuery query = session.CreateQuery(hql);
				AddParameters(parameters, query);
				IList graph = query.List();
				CurrentHqlGraph = new HqlResultGraph(graph, session);
				return CurrentHqlGraph;
			}
			catch (Exception ex)
			{
				if (logger.IsErrorEnabled)
					logger.Error("Could not run query: " + ex.ToString());
				if (ExceptionStackSerializable(ex))
					throw;
				else
					//Otherwise we lose all the exception info.
					throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// Walk the exception stack and make sure that all the exceptions 
		/// are seriliazble.
		/// </summary>
		private static bool ExceptionStackSerializable(Exception ex)
		{
			return ex.GetType().IsSerializable && (ex.InnerException == null || //If null, then it's true and the rest won't get evaluate.
				ExceptionStackSerializable(ex.InnerException));
		}

		public HqlResultGraph CurrentHqlGraph
		{
			get { return currentHqlGraph; }
			set
			{
				if (currentHqlGraph != null)
					currentHqlGraph.Dispose();
				currentHqlGraph = value;
			}
		}

		/// <summary>
		/// Gets the assembly filename, return the assembly name if not found.
		/// </summary>
		private string GetAssemblyFilename(string name)
		{
			foreach (string assembly in assemblies)
			{
				if (name == Path.GetFileNameWithoutExtension(assembly))
					return assembly;
			}
			return name;
		}

		public DataSet RunHqlAsRawSql(string hqlQuery, params TypedParameter[] parameters)
		{
			try
			{
				using (ISessionImplementor session = (ISessionImplementor) factory.OpenSession())
				{
					
						IList commands = HqlToCommandList(hqlQuery, TypedParameterToQueryParameter(parameters), session);
					    DataSet ds = new DataSet();
                        foreach (IDbCommand command in commands)
					    {
                            using (command)
                            {
                                command.Connection = session.Connection;
                                using (IDataReader reader = command.ExecuteReader())
                                {
                                    AddResultsToDataSet(reader, ds);
                                }
                            }
					    }
                        return ds;
				}
			}
			catch (Exception ex)
			{
				if (logger.IsErrorEnabled)
					logger.Error("Could not run query: " + ex.ToString());
				if (ExceptionStackSerializable(ex))
					throw;
				else
					//Otherwise we lose all the exception info.
					throw new Exception(ex.ToString());
			}
		}

	    private void AddResultsToDataSet(IDataReader reader, DataSet ds)
		{
			int ordinal;
			DataRow row;
			DataTable table;
		    table = CreateSchema(ds, reader.GetSchemaTable());
			while (reader.Read())
			{
				row = table.NewRow();
				foreach (DataColumn column in table.Columns)
				{
					ordinal = reader.GetOrdinal(column.ColumnName);
					row[column] = reader.GetValue(ordinal);
				}
				table.Rows.Add(row);
			}
		}

		private DataTable CreateSchema(DataSet ds, DataTable schemaTable)
		{
			if (schemaTable.Rows.Count == 0)
				throw new InvalidOperationException("Empty schema table! Fix this somehow!");
			string column, table, baseTableName = "BaseTableName";
			if (schemaTable.Rows[0].IsNull(baseTableName) == false)
				table = (string) schemaTable.Rows[0][baseTableName];
			else
				table = "Unknown table name";
			if (ds.Tables[table] == null)
				ds.Tables.Add(table);
			foreach (DataRow row in schemaTable.Rows)
			{
				column = (string) row["ColumnName"];
				if (ds.Tables[table].Columns[column] == null)
				{
					ds.Tables[table].Columns.Add(column);
					ds.Tables[table].Columns[column].DataType = (Type) row["DataType"];

				}
			}
			return ds.Tables[table];
		}

		public void Dispose()
		{
			if (currentHqlGraph != null)
				currentHqlGraph.Dispose();
		}

		private QueryParameters TypedParameterToQueryParameter(TypedParameter[] parameters)
		{
			QueryParameters qp = new QueryParameters(new IType[0], new object[0]);
			qp.NamedParameters = TypedParametersToIDictionary(parameters);
			return qp;
		}

		private static Hashtable TypedParametersToIDictionary(TypedParameter[] parameters)
		{
			Hashtable ht = new Hashtable(parameters.Length);
			foreach (TypedParameter parameter in parameters)
			{
				ht[parameter.Name] = new TypedValue(parameter.IType, parameter.Value);
			}
			return ht;
		}

	}
}