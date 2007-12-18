#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml.XPath;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Iesi.Collections.Generic;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Hql.Classic;
using NHibernate.Mapping;
using NHibernate.Type;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
	using System.Diagnostics;

	public class RemoteProject : MarshalByRefObject, IDisposable
	{
		private static readonly MethodInfo getQuery =
			Type.GetType("NHibernate.Impl.SessionFactoryImpl,NHibernate").GetMethod("GetQuery");

		#region Variables

		private static readonly ILog logger = LogManager.GetLogger(typeof(RemoteProject));

		private static readonly MethodInfo prepareQueryCommand =
			typeof(QueryTranslator).GetMethod("PrepareQueryCommand", BindingFlags.NonPublic | BindingFlags.Instance);

		private static List<Assembly> visited = new List<Assembly>();

		private readonly Configuration cfg;
		private readonly Hashtable loadedAssemblies = new Hashtable();
		private IList assemblies;
		private IList basePaths;
		private HqlResultGraph currentHqlGraph;
		private ISessionFactory factory;
		private bool pluralizeTableNames;
		private Settings settings;
		private readonly ISet<string> addedFiles = new HashedSet<string>();

		#endregion

		public RemoteProject()
		{
			cfg = new Configuration();
		}

		#region Properties

		public Configuration Cfg
		{
			get { return cfg; }
		}


		public SortedList MappingFilesCollection
		{
			get
			{
				SortedList list = new SortedList();

				foreach (PersistentClass model in cfg.ClassMappings)
				{
					string EntityName = model.MappedClass.Name;

					MappingEntity entity = new MappingEntity();
					entity.EntityName = EntityName;
					entity.TableName = model.Table.Name;

					foreach (Property property in model.PropertyCollection)
					{
						entity.AddProperty(property.Name, property.Type.Name, property.Type.IsEntityType,
										   property.Type.ReturnedClass.Name);
					}

					Property IdentityProperty = model.IdentifierProperty;
					entity.AddProperty(IdentityProperty.Name, IdentityProperty.Type.Name, false,
									   IdentityProperty.Type.ReturnedClass.Name);
					list.Add(entity.EntityName, entity);
				}

				return list;
			}
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

		#region IDisposable Members

		public void Dispose()
		{
			if (currentHqlGraph != null)
				currentHqlGraph.Dispose();
		}

		#endregion

		#endregion

		#region Constructors

		#endregion

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public string[] HqlToSql(string hqlQuery, IDictionary parameters)
		{
			using (ISession session = factory.OpenSession())
			{
				IList list = HqlToSqlList(hqlQuery,
										  CreateQueryParameters(parameters), (ISessionImplementor)session);
				ArrayList cmds = new ArrayList();
				foreach (string sqlString in list)
				{
					cmds.Add(sqlString);
					if (logger.IsDebugEnabled)
						logger.Debug("Resulting SQL: " + sqlString);
				}
				return (string[])cmds.ToArray(typeof(string));
			}
		}

		private QueryParameters CreateQueryParameters(IDictionary parameters)
		{
			QueryParameters qp = new QueryParameters(new IType[0], new object[0]);
			Hashtable ht = new Hashtable(parameters.Count);
			foreach (DictionaryEntry parameter in parameters)
			{
				TypedParameter tp = (TypedParameter)parameter.Value;
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
			IQueryTranslator[] queryTranslators =
				(IQueryTranslator[])getQuery.Invoke(factoryImplementor, new object[] { hqlQuery, false, null });
			IList commands = new ArrayList();
			foreach (IQueryTranslator translator in queryTranslators)
			{
				IDbCommand cmd = (IDbCommand)prepareQueryCommand.Invoke(translator, new object[] { qp, false, session });
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

		public void BuildInternalProject(IList assemblies, IList mappings, IList configurations, IList basePaths)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			this.basePaths = basePaths;
			this.assemblies = assemblies;
			basePaths.Add(AppDomain.CurrentDomain.BaseDirectory);
			LoadConfigurations(configurations);
			LoadAssemblies(assemblies);
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
				string[] resourceNames = assembly.GetManifestResourceNames();
				foreach (string name in resourceNames)
				{
					if (name.EndsWith("hbm.xml") == false)
						continue;
					addedFiles.Add(name);
					cfg.AddResource(name, assembly);
				}

				bool isActiveRecord = false;

				foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
				{
					//doing it like this because want to keep it version safe
					Assembly arAsm = GetActiveRecordAsembly(assembly);
					if (arAsm != null)
					{
						arAsm.GetType("Castle.ActiveRecord.Framework.Internal.ActiveRecordModel")
							.GetField("pluralizeTableNames", BindingFlags.Static | BindingFlags.NonPublic)
							.SetValue(null, pluralizeTableNames);
						AddActiveRecordAssembly(assembly, arAsm);
						isActiveRecord = true;
						break;
					}
				}

				if (!isActiveRecord)
				{
					visited.Clear();

					foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
					{
						//doing it like this because want to keep it version safe
						Assembly maAsm = GetMappingAttributesAssembly(assembly);
						if (maAsm != null)
						{
							AddMappingAttributesAssembly(assembly, maAsm);
							break;
						}
					}
				}
			}
		}

		private void AddActiveRecordAssembly(Assembly asm, Assembly activeRecordAssembly)
		{
			if (activeRecordAssembly == null)
			{
				throw new InvalidOperationException(
					string.Format("Could not find Active Record assembly referenced from {0}", asm));
			}
			object activeRecordModelBuilder =
				Activator.CreateInstance(
					activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.ActiveRecordModelBuilder"));
			ArrayList models = new ArrayList();
			Type[] asmTypes;
			try
			{
				asmTypes = asm.GetTypes();
			}
			catch (ReflectionTypeLoadException ex) //for types that inherit from other assemblies
			{
				asmTypes = ex.Types;
			}

			foreach (Type type in asmTypes)
			{
				if (IsActiveRecordType(type) == false)
					continue;
				object model = Invoke(activeRecordModelBuilder, "Create", type);
				if (model == null)
					continue;
				models.Add(model);
			}

			object graphConnectorVisitor =
				Activator.CreateInstance(
					activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.GraphConnectorVisitor"),
					new object[] { Get(activeRecordModelBuilder, "Models") });

			Invoke(graphConnectorVisitor, "VisitNodes", models);

			object semanticVisitor =
				Activator.CreateInstance(
					activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.SemanticVerifierVisitor"),
					new object[] { Get(activeRecordModelBuilder, "Models") });

			Invoke(semanticVisitor, "VisitNodes", models);

			foreach (object model in models)
			{
				bool isNestedType = (bool)Get(model, "IsNestedType");
				bool isDiscriminatorSubClass = (bool)Get(model, "IsDiscriminatorSubClass");
				bool isJoinedSubClass = (bool)Get(model, "IsJoinedSubClass");

				if (!isNestedType && !isDiscriminatorSubClass && !isJoinedSubClass)
				{
					object xmlVisitor =
						Activator.CreateInstance(
							activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.XmlGenerationVisitor"));

					Invoke(xmlVisitor, "CreateXml", model);

					cfg.AddXmlString((string)Get(xmlVisitor, "Xml"));
				}
			}

			Type assemblyXmlGeneratorType =
				activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.AssemblyXmlGenerator");
			if (assemblyXmlGeneratorType != null)
			//we may have a slightly old version of ActiveRecord, that doesn't have this
			{
				object assemblyXmlGenerator =
					Activator.CreateInstance(
						assemblyXmlGeneratorType,
						new object[] { });
				string[] xmls = (string[])Invoke(assemblyXmlGenerator, "CreateXmlConfigurations", asm);
				foreach (string xml in xmls)
				{
					cfg.AddXmlString(xml);
				}
			}
		}

		private static bool IsActiveRecordType(Type type)
		{
			foreach (object customAttribute in type.GetCustomAttributes(false))
			{
				if (customAttribute.GetType().Name == "ActiveRecordAttribute")
					return true;
			}
			return false;
		}

		public static object Invoke(object obj, string name, params object[] args)
		{
			return obj.GetType().GetMethod(name).Invoke(obj, args);
		}


		public static object Get(object obj, string name)
		{
			return obj.GetType().GetProperty(name).GetValue(obj, null);
		}

		public static void Set(object obj, string name, object value)
		{
			obj.GetType().GetProperty(name).SetValue(obj, value, null);
		}

		/// <summary>
		/// This is needed to make sure that we work with different versions of Active Record
		/// </summary>
		private static Assembly GetActiveRecordAsembly(Assembly assembly)
		{
			if (visited.Contains(assembly))
				return null;
			visited.Add(assembly);
			Type type = assembly.GetType("Castle.ActiveRecord.ActiveRecordBase", false);
			if (type != null)
				return type.Assembly;
			foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
			{
				Assembly refAsm = Assembly.Load(assemblyName);
				Assembly result = GetActiveRecordAsembly(refAsm);
				if (result != null)
					return result;
			}
			return null;
		}

		private static Assembly GetMappingAttributesAssembly(Assembly assembly)
		{
			if (visited.Contains(assembly))
			{
				return null;
			}

			visited.Add(assembly);

			Type type = assembly.GetType("NHibernate.Mapping.Attributes.ClassAttribute", false);
			if (type != null)
			{
				return type.Assembly;
			}

			foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
			{
				Assembly refAsm = Assembly.Load(assemblyName);
				Assembly result = GetMappingAttributesAssembly(refAsm);

				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		private void AddMappingAttributesAssembly(Assembly asm, Assembly mappingAttributesAssembly)
		{
			if (mappingAttributesAssembly == null)
			{
				throw new InvalidOperationException(
					string.Format("Could not find Mapping.Attributes assembly referenced from {0}", asm));
			}

			object hbmSerializer = Activator.CreateInstance(mappingAttributesAssembly.GetType("NHibernate.Mapping.Attributes.HbmSerializer"));

			Set(hbmSerializer, "Validate", true);

			MethodInfo serializeMethod = hbmSerializer.GetType().GetMethod("Serialize", new Type[] { typeof(Assembly) });

			Stream stream = serializeMethod.Invoke(hbmSerializer, new object[] { asm }) as Stream;

			if (stream == null)
			{
				throw new InvalidOperationException(String.Format("Could not serialize assembly {0}", asm.FullName));
			}

			cfg.AddInputStream(stream);
		}

		private void LoadMappings(IList mappings)
		{
			foreach (string mapping in mappings)
			{
				bool shouldSkip = false;
				foreach (string addedFile in addedFiles)
				{
					if (addedFile.EndsWith(Path.GetFileName(mapping)))
					{
						if (logger.IsDebugEnabled)
							logger.Debug("Skipping mapping file: " + mapping + " because it was already loaded");
						shouldSkip = true;
						break;
					}
				}
				if (shouldSkip)
					continue;
				if (logger.IsDebugEnabled)
					logger.Debug("Loading mapping: " + mapping);
				cfg.AddXmlFile(mapping);
				addedFiles.Add(Path.GetFileName(mapping));
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
			string app_config = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			LoadFromActiveRecordConfig(app_config);
			if (cfg.Properties[Environment.ConnectionStringName] != null)
			{
				throw new ConfigurationErrorsException("You cannot use " + Environment.ConnectionStringName +
													   " with NHibernate Query Analyzer, please use " +
													   Environment.ConnectionString);
			}
		}

		private void LoadFromActiveRecordConfig(string configuration)
		{
			XPathDocument xdoc = new XPathDocument(configuration);
			foreach (XPathNavigator node in xdoc.CreateNavigator().Select("/configuration/activerecord/config/add"))
			{
				string key = node.GetAttribute("key", "");
				string val = node.GetAttribute("value", "");
				cfg.Properties[key] = val;
			}
			XPathNavigator pluralize =
				xdoc.CreateNavigator().SelectSingleNode("/configuration/activerecord/@pluralizeTableNames");
			if (pluralize == null)
				return;
			pluralizeTableNames = "true".Equals(pluralize.Value, StringComparison.InvariantCultureIgnoreCase);
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
			Assembly asm = (Assembly)loadedAssemblies[assemblyName];
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
					logger.Error("Could not run query.", ex);
				if (ExceptionStackSerializable(ex))
					throw;
				else
					//Otherwise we lose all the exception info.
					throw new Exception(ex.ToString());
			}
		}

		/// <summary>
		/// Walk the exception stack and make sure that all the exceptions 
		/// are seriliazble.
		/// </summary>
		private static bool ExceptionStackSerializable(Exception ex)
		{
			return
				ex.GetType().IsSerializable &&
				(ex.InnerException == null || //If null, then it's true and the rest won't get evaluate.
				 ExceptionStackSerializable(ex.InnerException));
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
				using (ISession session = factory.OpenSession())
				{
					IList commands = HqlToCommandList(hqlQuery, TypedParameterToQueryParameter(parameters), (ISessionImplementor)session);
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
				table = (string)schemaTable.Rows[0][baseTableName];
			else
				table = "Unknown table name";
			if (ds.Tables[table] == null)
				ds.Tables.Add(table);
			foreach (DataRow row in schemaTable.Rows)
			{
				column = (string)row["ColumnName"];
				if (ds.Tables[table].Columns[column] == null)
				{
					ds.Tables[table].Columns.Add(column);
					ds.Tables[table].Columns[column].DataType = (Type)row["DataType"];
				}
			}
			return ds.Tables[table];
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
