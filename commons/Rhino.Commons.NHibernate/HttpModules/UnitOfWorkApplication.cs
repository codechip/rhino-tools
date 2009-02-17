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
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using Castle.Windsor;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
    public class UnitOfWorkApplication : HttpApplication, IContainerAccessor
    {
        public const string CurrentLongConversationKey = "CurrentLongConversation.Key";
        public const string CurrentNHibernateSessionKey = "CurrentNHibernateSession.Key";

        private FileSystemWatcher watcher;

        public UnitOfWorkApplication()
        {
            PreRequestHandlerExecute += UnitOfWorkApplication_BeginRequest;
            PostRequestHandlerExecute += UnitOfWorkApplication_EndRequest;
        }

        public virtual void UnitOfWorkApplication_BeginRequest(object sender, EventArgs e)
        {
            if (IoC.IsInitialized == false)
                InitializeContainer(this);

			bool loadedConversation = false;
            if (IsAspSessionAvailable)
            {
            	loadedConversation = LongConversationManager.LoadConversation();
            }

			if (!loadedConversation)
            {
                UnitOfWork.Start();
            }
        }

        private static bool IsAspSessionAvailable
        {
            get { return HttpContext.Current.Session != null; }
        }

        public virtual void UnitOfWorkApplication_EndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Server.GetLastError() == null && UnitOfWork.InLongConversation)
            {
                UnitOfWork.CurrentSession.Disconnect();
                if (!IsAspSessionAvailable)
                {
                    throw new InvalidOperationException(
                        "Session must be enabled when using Long Conversations! If you are using web services, make sure to use [WebMethod(EnabledSession=true)]");
                }
				LongConversationManager.SaveConversation();
            }
            else
            {
                if (UnitOfWork.IsStarted)
                    UnitOfWork.Current.Dispose();
            }
        }

        public IWindsorContainer Container
        {
            get
            {
				if (IoC.IsInitialized == false)
					InitializeContainer(this);
				return IoC.Container;
            }
        }

        public virtual void Application_Start(object sender, EventArgs e)
        {
            InitializeContainer(this);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void InitializeContainer(UnitOfWorkApplication self)
        {
            if (IoC.IsInitialized)
                return;
            self.CreateContainer();
        }

        public virtual void Application_End(object sender, EventArgs e)
        {
            if (Container != null) //can happen if this isn't the first app
            {
                IoC.Reset(Container);
                Container.Dispose();
            }
        }

        public override void Dispose()
        {
			// we can't unregister to those, we get invalid operation exeption
			// when we try that.

			//BeginRequest -= UnitOfWorkApplication_BeginRequest;
			//EndRequest -= UnitOfWorkApplication_EndRequest;
            if (watcher != null)
                watcher.Dispose();
        }

        private void CreateContainer()
        {
            string windsorConfig = GetWindsorConfig();

        	FileSystemEventHandler resetIoC = delegate 
			{
				if (IoC.IsInitialized)
				{
					IoC.Container.Dispose();
				}
				IoC.Reset(); 
			};
            watcher = new FileSystemWatcher(Path.GetDirectoryName(windsorConfig));
            watcher.Filter = Path.GetFileName(windsorConfig);
            watcher.Created += resetIoC;
            watcher.Changed += resetIoC;
            watcher.Deleted += resetIoC;

        	IWindsorContainer container = CreateContainer(windsorConfig);
        	IoC.Initialize(container);

            watcher.EnableRaisingEvents = true;
        }

    	private static string GetWindsorConfig()
    	{
    		string windsorConfig = Settings.Default.WindsorConfig;
    		if (!Path.IsPathRooted(windsorConfig))
    		{
    			//In ASP.Net apps, the current directory and the base path are NOT the same.
    			windsorConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, windsorConfig);
    		}
    		string[] extensions = {".config", ".boo" };
    		foreach (string extension in extensions)
    		{
    			string path = windsorConfig+extension;
    			if(File.Exists(path))
    			{
    				windsorConfig = path;
    				break;
    			}
    		}
    		return windsorConfig;
    	}

    	protected virtual IWindsorContainer CreateContainer(string windsorConfig)
        {
            return new RhinoContainer(windsorConfig);
        }
    }
}
