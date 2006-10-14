using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Finisar.SQLite;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using Environment = System.Environment;

namespace Ayende.NHibernateQueryAnalyzer.Model
{
	public class ProjectsRepository : IProjectsRepository
	{
		#region Variables

		private ISessionFactory factory;
		private ISession session;
		private static ILog logger = LogManager.GetLogger(typeof (ProjectsRepository));

		#endregion 
		
		#region Properties
		/// <summary>
		/// Gets the current session (create one if it doesn't exist).
		/// Since we're using sqlite for the database, the single connection
		/// is not important to close, so it'll always be open.
		/// </summary>
		private ISession Session
		{
			get
			{
				if (session == null)
				{
					if (logger.IsDebugEnabled)
						logger.Debug("Openning a new session");
					session = factory.OpenSession();
				}
				return session;
			}
		}

		#endregion 

		#region Project Management

		/// <summary>
		/// Removes the project and all its associate data from NQA's database.
		/// </summary>
		/// <param name="project">The project to remove.</param>
		public void RemoveProject(Project project)
		{
			if (logger.IsInfoEnabled)
				logger.Info("Removing project: " + project.Name);
			Session.Delete(project);
			Session.Flush();
		}

		public Project GetProjectById(int id)
		{
			return (Project) Session.Get(typeof (Project), id);
		}

		public Project GetProjectByName(string projectName)
		{
			IQuery query = Session.CreateQuery("from Project prj where prj.Name = :name").SetString("name", projectName);
			IList list = query.List();
			if (list.Count > 0)
				return (Project) list[0];
			else
				return null;
		}

		public IList GetProjectsStartingWith(string similarProjectName)
		{
			IQuery query = Session.CreateQuery("from Project prj where prj.Name like :name").SetString("name", similarProjectName + "%");
			return query.List();

		}

		public Project CreateProject(string projectName)
		{
			return new Project(projectName);
		}

		public void SaveProject(Project prj)
		{
			if (prj.Id == 0)
				Session.Save(prj);
			else
				Session.Update(prj);
			Session.Flush();
		}

		public void RemoveFromCache(Project current)
		{
			Session.Evict(current);
		}

		public IList GetAllProejcts()
		{
			if (logger.IsDebugEnabled) logger.Debug("GetAllProjects called");
			return Session.CreateCriteria(typeof (Project)).List();
		}

		#endregion 

		#region c'tors

		public ProjectsRepository(Configuration cfg) : this(cfg, new DataBaseSetup())
		{}

		public ProjectsRepository(Configuration cfg, DataBaseSetup dataBaseSetup)
		{
			dataBaseSetup.SetupDataBase(cfg);
			factory = cfg.BuildSessionFactory();
			if (logger.IsDebugEnabled)
				logger.Debug("Application data factory created successfully.");
		}

		#endregion 

		#region Context class

		public class DataBaseSetup : IDataBaseSetupImpl
		{
			/// <summary>
			/// Ensures the database file exists.
			/// If it doesn't exist, it and the tables will be created
			/// </summary>
			public virtual void EnsureDatabaseFileExists(string path)
			{
				if (!File.Exists(path))
				{
					if (logger.IsDebugEnabled)
						logger.Debug("Data file does not exist, creating from scratch.");
					CreateDB(path);
				}
			}

			/// <summary>
			/// Gets the data source file path.
			/// </summary>
			public virtual string GetDataSourceFilePath(string cnstr)
			{
				Regex dataSourceRegEx = new Regex(@".*Data\s+Source\s*=(?<DataSource>([^;]*))");

				Match m = dataSourceRegEx.Match(cnstr);
				string path = m.Groups["DataSource"].Value;
				if (logger.IsDebugEnabled)
					logger.Debug("Data file is: " + path);
				return path;
			}

			/// <summary>
			/// Expend any envirnment variables in the connection string.
			/// </summary>
			public virtual string ExpandEnvironmentVariablesInConnectionString(Configuration cfg)
			{
				string connectionStringProeprty = "hibernate.connection.connection_string";
				string cnstr = cfg.GetProperty(connectionStringProeprty);
				if (logger.IsDebugEnabled)
					logger.Debug("Connection String is:" + cnstr);
				cnstr = Environment.ExpandEnvironmentVariables(cnstr);
				if (logger.IsDebugEnabled)
					logger.Debug("Expanded connection string is: " + cnstr);
				cfg.SetProperty(connectionStringProeprty, cnstr);
				return cnstr;
			}

			private void CreateDB(string path)
			{
				if (logger.IsDebugEnabled)
					logger.Debug("Creating database at: " + path);
				string folder = Path.GetDirectoryName(path);
				if (!Directory.Exists(folder))
				{
					if (logger.IsDebugEnabled)
						logger.Debug("Directory does not exist, creating directory; " + folder);
					Directory.CreateDirectory(folder);
				}
				using (SQLiteConnection con = new SQLiteConnection("Data Source=" + path + ";Version=3;New=True"))
					CreateDatabase(con);
			}

			public virtual void CreateDatabase(IDbConnection con)
			{
				con.Open();
				if (logger.IsDebugEnabled)
					logger.Debug("Database created successfully.");
				using (IDbCommand command = con.CreateCommand())
				{
					using (TextReader sql = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Ayende.NHibernateQueryAnalyzer.Core.Setup.sql")))
					{
						//This is a hack to overcome the fact that SQLLiteCommand doesn't support 
						//Multiply commands.
						foreach (string sqlCommand in sql.ReadToEnd().Split('$'))
						{
							if (logger.IsDebugEnabled)
								logger.Debug("Executing command: " + sqlCommand);
							command.CommandText = sqlCommand;
							command.ExecuteNonQuery();
						}
					}
				}
			}

			public void SetupDataBase(Configuration cfg)
			{
				string cnstr = ExpandEnvironmentVariablesInConnectionString(cfg);
				string path = GetDataSourceFilePath(cnstr);
				EnsureDatabaseFileExists(path);
			}
		}

		#endregion

		#region Query

		public void SaveQuery(Query q)
		{
			if (q.Id == 0)
				Session.Save(q);
			else
				Session.Update(q);
			Session.Flush();
		}

		#endregion 
	}

}