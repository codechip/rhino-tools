namespace Rhino.DSL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.IO;

    /// <summary>
    /// Handles the storage requirements for the DSL from a file system.
    /// </summary>
    public class FileSystemDslEngineStorage : IDslEngineStorage
    {
        private readonly Dictionary<string, FileSystemWatcher> pathToFileWatchers = new Dictionary<string, FileSystemWatcher>();

        /// <summary>
        /// Create a compiler input from the URL.
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The compiler input</returns>
        public virtual ICompilerInput CreateInput(string url)
        {
            return new FileInput(url);
        }

        /// <summary>
        /// The file name format of this DSL
        /// </summary>
        public virtual string FileNameFormat
        {
            get
            {
                return "*.boo";
            }
        }

        /// <summary>
        /// Will retrieve all the _canonised_ urls from the given directory that
        /// this Dsl Engine can process.
        /// </summary>
        public virtual string[] GetMatchingUrlsIn(string parentPath, ref string url)
        {
            url = Path.GetFullPath(Path.Combine(parentPath, url));
            string directory = url;
            if (Directory.Exists(directory) == false)
                directory = Path.GetDirectoryName(directory);
            string[] files = Directory.GetFiles(directory, FileNameFormat);
            Array.Sort(files);
            return files;
        }

        /// <summary>
        /// Compares the two urls
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual int CompareUrls(string x, string y)
        {
            return x.CompareTo(y);
        }

        /// <summary>
        /// Will call the action delegate when any of the specified urls are changed.
        /// Note that for a single logical change several calls may be made.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <param name="action">The action.</param>
        public virtual void NotifyOnChange(IEnumerable<string> urls, Action<string> action)
        {
            lock (pathToFileWatchers)
            {
                string[] commonPaths = GatherCommonPaths(urls);
                foreach (string path in commonPaths)
                {
                    FileSystemWatcher watcher;
                    if(pathToFileWatchers.TryGetValue(path, out watcher)==false)
                    {
                        pathToFileWatchers[path] = watcher = new FileSystemWatcher(path, FileNameFormat);
                        watcher.EnableRaisingEvents = true;
                    }
                    watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                    {
                        action(e.FullPath);
                    };
                }
            }
        }

        private static string[] GatherCommonPaths(IEnumerable<string> urls)
        {
            List<string> paths = new List<string>();
            foreach (string url in urls)
            {
                string path = Path.GetDirectoryName(url);
                if(paths.Contains(path)==false)
                    paths.Add(path);
            }
            return paths.ToArray();
        }

        /// <summary>
        /// Determines whether the URL is included in the specified urls
        /// in the given parent path
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <param name="parentPath">The parent oath.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// 	<c>true</c> if [is URL include in] [the specified urls]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsUrlIncludeIn(string[] urls, string parentPath, string url)
        {
            string path = Path.GetFullPath(Path.Combine(parentPath, url));
            Uri pathUrl = new Uri(path);
            return Array.Exists(urls, delegate(string urlInArray)
            {
                return new Uri(urlInArray).Equals(pathUrl);
            });
        }


        /// <summary>
        /// Gets the type name from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public virtual string GetTypeNameFromUrl(string url)
        {
            return Path.GetFileNameWithoutExtension(url);   
        }
        
        /// <summary>
        /// Determains whatever the given url is a valid script url.
        /// </summary>
        public virtual bool IsValidScriptUrl(string url)
        {
			return File.Exists(url);
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            lock (pathToFileWatchers)
            {
                foreach (FileSystemWatcher watcher in pathToFileWatchers.Values)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
        }
    }
}