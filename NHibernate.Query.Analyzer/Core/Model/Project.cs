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
using System.Data;
using System.IO;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Exceptions;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Iesi.Collections;
using log4net;

namespace Ayende.NHibernateQueryAnalyzer.Core.Model
{
    public class Project : IDisposable
    {
        #region Variables

        private AppDomainSetup appDomainSetup;
        protected bool isProjectBuilt = false;
        protected IList mappings;
        protected IList configurations;
        protected IList assemblies;
        private Context context;
        protected RemoteProject remoteProject;
        private ILog logger = LogManager.GetLogger(typeof(Project));
        private ISet queries = null;
        private string name;
        private int id;
        private IList files = null;
        private AppDomain appDomain;
        private IList basePaths = new ArrayList();

        #endregion

        #region Properties

        public virtual bool IsProjectBuilt
        {
            get { return isProjectBuilt; }
        }

        public virtual AppDomainSetup AppDomainSetup
        {
            get { return appDomainSetup; }
        }

        /// <summary>
        /// Return a list of all the queries saved for this project.
        /// Key: Query name
        /// Value: Query text
        /// </summary>
        public virtual ISet Queries
        {
            get
            {
                if (queries == null)
                    queries = new ListSet();
                return queries;
            }
            set { queries = value; }
        }

        /// <summary>
        /// Gets the id of the project.
        /// </summary>
        public virtual int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets or sets the name of the project
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets the collection of files for this project.
        /// The elements of the Files collections are strings (paths to the files.)
        /// </summary>
        /// <value></value>
        public virtual IList Files
        {
            get
            {
                if (files == null)
                    files = new ArrayList();
                return files;
            }
        }


        public virtual bool IsSessionOpen
        {
            get { return remoteProject.CurrentHqlGraph != null && remoteProject.CurrentHqlGraph.IsSessionOpen; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Project"/> instance.
        /// This is only called from the public virtual ctor or from NHibernate
        /// </summary>
        public Project()
            : this(new Context())
        {
            if (logger.IsDebugEnabled)
                logger.Debug("Creating project {private ctor() called} - NHibernate creating the project");
        }

        private Project(Context context)
        {
            id = 0;
            this.context = context;
            mappings = new ArrayList();
            configurations = new ArrayList();
            assemblies = new ArrayList();
            appDomainSetup = new AppDomainSetup();
        }

        /// <summary>
        /// Creates a new named <see cref="Project"/> instance.
        /// </summary>
        /// <param name="name">Name.</param>
        public Project(string name)
            : this(name, new Context())
        {
        }

        /// <summary>
        /// Creates a new named <see cref="Project"/> instance.
        /// </summary>
        /// <param name="name">Name.</param>
        public Project(string name, Context context)
            : this(context)
        {
            if (logger.IsDebugEnabled)
                logger.Debug("Creating new project: '" + name + "'");
            this.name = name;
        }

        #endregion

        #region Add / Remove files

        /// <summary>
        /// Adds the file to the current project. 
        /// Will skip files already existing.
        /// Fire the OnFileAdd event.
        /// </summary>
        /// <param name="file">Path to the spesified file.</param>
        public virtual void AddFile(string file)
        {
            if (IsProjectBuilt)
                throw new InvalidOperationException("Can't add files to a project that has been built, call reset and try again");
            string fileName = new FileInfo(file).FullName;
            if (logger.IsDebugEnabled)
                logger.Debug("Adding file: " + fileName);
            if (Files.Contains(fileName))
            {
                if (logger.IsDebugEnabled)
                    logger.Debug("File already exist, ignoring...");
                return; //No need to do anything as we already handled it.
            }
            Files.Add(fileName);
        }

        public virtual void RemoveFile(string file)
        {
            if (IsProjectBuilt)
                throw new InvalidOperationException("Can't remove files to a project that has been built, call reset and try again");
            string fileName = new FileInfo(file).FullName;
            if (logger.IsDebugEnabled)
                logger.Debug("Removing file: " + fileName);
            files.Remove(fileName);
            if (logger.IsDebugEnabled)
                logger.Debug("File removed successfully, resetting project Configuration and SessionFactory");
        }

        #endregion

        #region Build Project

        /// <summary>
        /// Prepare the project for work.
        /// It will create an AppDomain, load the required files and prepare the
        /// project for real work.
        /// </summary>
        public virtual void BuildProject()
        {
            try
            {
                HandleFiles();
                AppDomainSetup.ShadowCopyFiles = "true";
                AppDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                AppDomainSetup.PrivateBinPath = CreatePrivateBinPath();
                appDomain = AppDomain.CreateDomain(Name, null, AppDomainSetup);
                //This is so the domain can pick up the stuff that it needs like the project loaders
                //and nhibernate.
                remoteProject = (RemoteProject)appDomain.CreateInstanceFromAndUnwrap(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ayende.NHibernateQueryAnalyzer.ProjectLoader.dll"), 
                    "Ayende.NHibernateQueryAnalyzer.ProjectLoader.RemoteProject");
                remoteProject.BuildInternalProject(assemblies, mappings, configurations, basePaths);
                isProjectBuilt = true;
            }
            catch (Exception)
            {
                isProjectBuilt = false;
                throw;
            }
        }

        private string CreatePrivateBinPath()
        {
            string[] dirs = new string[assemblies.Count];
            int i=0;
            foreach (string asm in assemblies)
            {
                dirs[i] = Path.GetDirectoryName(asm);
                i++;
            }
            return string.Join(";", dirs);
        }

        public virtual void HandleFiles()
        {
            foreach (string file in files)
            {
                context.HandleAddedFile(file, this);
            }
        }

        public virtual void ResetProject()
        {
            Dispose();
        }

        #endregion

        #region Context Class

        public class Context
        {
            public interface IFileAdd
            {
                void AddFile(Project toProject);
            }

            /// <summary>
            /// Check what the file type is (based on the file's extention) and call the appropriate method.
            /// </summary>
            /// <exception cref="FileLoadingException">Thrown if the file extention is not one that <see cref="HandleAddedFile"/> knows how to deal with or when processing the file caused an error.</exception>
            /// <param name="file">Path to the new added file</param>
            public virtual void HandleAddedFile(string file, Project prj)
            {
                try
                {
                    if (File.Exists(file) == false)
                        throw new FileLoadingException("File " + Path.GetFullPath(file) + " does not exists");
                    CreateFileAdd(file).AddFile(prj);
                }
                catch (FileLoadingException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new FileLoadingException("Unable to load file: " + file, e);
                }
            }

            public virtual IFileAdd CreateFileAdd(string filename)
            {
                return CreateFileAdd(filename, filename);
            }

            public virtual IFileAdd CreateFileAdd(string filename, string path)
            {
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".xml":
                        //recurse and then check if it's .hbm or .cfg file.
                        return CreateFileAdd(Path.GetFileNameWithoutExtension(filename), path);
                    case ".hbm":
                        //because we truncate it earlier
                        return new AddMapping(path);
                    case ".cfg":
                        //because we truncate it earlier
                        return new AddConfiguration(path);
                    case ".exe":
                    case ".dll":
                        return new AddAssembly(path);
                    case ".config":
                        return new AddAppConfig(path);
                    default:
                        return new AddUnkown(path);
                }
            }

            public class AddUnkown : IFileAdd
            {
                private string file;

                public AddUnkown(string file)
                {
                    this.file = file;
                }

                public virtual void AddFile(Project parent)
                {
                    if (parent.logger.IsErrorEnabled)
                        parent.logger.Error("File " + file + " is not an known file.");
                    throw new UnknownFileTypeException(file);
                }
            }

            public class AddMapping : IFileAdd
            {
                private string file;

                public AddMapping(string file)
                {
                    this.file = file;
                }

                public virtual void AddFile(Project toProject)
                {
                    if (toProject.logger.IsInfoEnabled)
                        toProject.logger.Info("Added mapping file: " + file);
                    toProject.mappings.Add(file);
                }
            }

            /// <summary>
            /// Adds the configuration (cfg.xml) file to the project Configuration
            /// </summary>
            public class AddConfiguration : IFileAdd
            {
                private string file;

                public AddConfiguration(string file)
                {
                    this.file = file;
                }

                public virtual void AddFile(Project toProject)
                {
                    if (toProject.logger.IsInfoEnabled)
                        toProject.logger.Info("Added configuration file: " + file);
                    toProject.configurations.Add(file);
                }
            }

            /// <summary>
            /// Adds the assembly, this will read it to memory and then load it.
            /// </summary>
            public class AddAssembly : IFileAdd
            {
                private string file;

                public AddAssembly(string file)
                {
                    this.file = file;
                }

                public virtual void AddFile(Project toProject)
                {
                    if (toProject.logger.IsInfoEnabled)
                        toProject.logger.Info("Adding assembly: " + file);
                    toProject.AddBasePaths(Path.GetDirectoryName(file));
                    toProject.assemblies.Add(file);
                }
            }

            public class AddAppConfig : IFileAdd
            {
                private string file;

                public AddAppConfig(string file)
                {
                    this.file = file;
                }

                public virtual void AddFile(Project toProject)
                {
                    if (toProject.logger.IsInfoEnabled)
                        toProject.logger.Info("Added " + file + " as the config for project " + toProject.Name);
                    toProject.AppDomainSetup.ConfigurationFile = file;
                }
            }

        }

        #endregion

        #region IDisposable implementation

        public virtual void Dispose()
        {
            if (remoteProject != null)
                remoteProject.Dispose();
            remoteProject = null;
            if (appDomain != null)
                AppDomain.Unload(appDomain);
            appDomain = null;
            isProjectBuilt = false;
        }

        #endregion

        #region Database Methods

        /// <summary>
        /// Translate an HQL query into a SQL in as similar a way to the way
        /// NHibernate does it as possible
        /// </summary>
        /// <param name="hqlQuery">HQL query.</param>
        /// <returns>The SQL queries (more then one query possible) that NHibernate issue on this call</returns>
        public virtual string[] HqlToSql(string hqlQuery, IDictionary parameters)
        {
            if (!IsProjectBuilt)
                throw new InvalidOperationException("Can't translate queries if the project was not built.");
            try
            {
                return remoteProject.HqlToSql(hqlQuery, parameters);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public virtual NHibernate.Cfg.Configuration NHibernateConfiguration
        {
            get
            {
                return remoteProject.Cfg;
            }
        }

        public virtual SortedList MappingFiles
        {
            get { return remoteProject.MappingFilesCollection; }
        }

        public virtual IList RunHql(string hql, params TypedParameter[] parameters)
        {
            if (!isProjectBuilt)
                throw new InvalidOperationException("Can't run queries if the project is not built.");
            if (logger.IsInfoEnabled)
                logger.Info("Running HQL Query: " + hql);
            try
            {
                return remoteProject.RunHql(hql, parameters).RemoteGraph;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public virtual DataSet RunHqlAsRawSql(string hqlQuery, params TypedParameter[] parameters)
        {
            if (!isProjectBuilt)
                throw new InvalidOperationException("Can't run queries if the project is not built.");
            if (logger.IsInfoEnabled)
                logger.Info("Running Hql Query as raw sql: " + hqlQuery);
            try
            {
                return remoteProject.RunHqlAsRawSql(hqlQuery, parameters);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public virtual Context Conext
        {
            get { return context; }
        }

        public virtual AppDomain AppDomain
        {
            get { return appDomain; }
        }

        public virtual Query GetQueryWithName(string name)
        {
            foreach (Query query in Queries)
            {
                if (query.Name == name)
                {
                    return query;
                }
            }
            return null;
        }

        public virtual int AddQuery(Query query)
        {
            query.OwnerProject = this;
            Queries.Add(query);
            return Queries.Count;
        }

        public virtual void RemoveQuery(Query oldQuery)
        {
            oldQuery.OwnerProject = null;
            Queries.Remove(oldQuery);
        }

        #endregion

        public virtual void AddBasePaths(string path)
        {
            if (basePaths.Contains(path) == false)
            {
                basePaths.Add(path);
            }
        }
    }
}
