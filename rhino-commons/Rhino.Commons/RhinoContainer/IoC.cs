using System;
using System.Collections.Generic;
using System.Web;
using Castle.Windsor;

namespace Rhino.Commons
{
    public static class IoC
    {
        private static IDictionary<IWindsorContainer, HttpApplicationState> container2Application = new Dictionary<IWindsorContainer, HttpApplicationState>();
        private static IWindsorContainer container;
        private static object LocalContainerKey = new object();
        private static string GlobalContainerKey = "Global.Container.Key";

        public static void Initialize(IWindsorContainer windsorContainer)
        {
            GlobalContainer = windsorContainer;
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Container.Resolve<T>(name);
        }

        public static IWindsorContainer Container
        {
            get
            {
                IWindsorContainer result = LocalContainer ?? GlobalContainer;
                if (result == null)
                    throw new InvalidOperationException("The container has not been initialized!");
                return result;
            }
        }

        private static IWindsorContainer LocalContainer
        {
            get { return Local.Data[LocalContainerKey] as IWindsorContainer; }
            set { Local.Data[LocalContainerKey] = value; }
        }

        public static bool IsInitialized
        {
            get { return GlobalContainer != null; }
        }

        internal static IWindsorContainer GlobalContainer
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Application[GlobalContainerKey] as IWindsorContainer;
                }
                return container;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    lock (container2Application)
                    {
                        container2Application[value] = HttpContext.Current.Application;
                        HttpContext.Current.Application[GlobalContainerKey] = value;
                    }
                }
                container = value;
            }
        }

        /// <summary>
        /// This allows you to override the global container locally
        /// Useful for scenarios where you are replacing the global container
        /// but needs to do some initializing that relies on it.
        /// </summary>
        /// <param name="localContainer"></param>
        /// <returns></returns>
        public static IDisposable UseLocalContainer(IWindsorContainer localContainer)
        {
            LocalContainer = localContainer;
            return new DisposableAction(delegate
            {
                LocalContainer = null;
            });
        }

        public static void Reset(IWindsorContainer containerToReset)
        {
			if(containerToReset==null)
				return;
            lock (container2Application)
            {
                if (container2Application.ContainsKey(containerToReset))
                {
                    container2Application[containerToReset].Remove(GlobalContainerKey);
                }
                container2Application.Remove(containerToReset);
            }
            if (ReferenceEquals(GlobalContainer, containerToReset))
            {
                GlobalContainer = null;
            }
        }
    }
}