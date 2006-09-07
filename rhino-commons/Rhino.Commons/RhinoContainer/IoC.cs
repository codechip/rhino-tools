using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;

namespace Rhino.Commons
{
    public static class IoC
    {
        private static IWindsorContainer container;
        private static object LocalContainerKey = new object();

        public static void Initialize(IWindsorContainer windsorContainer)
        {
            IoC.container = windsorContainer;
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
                IWindsorContainer result = LocalContainer ?? container;
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
            if (ReferenceEquals(container, containerToReset))
            {
                container = null;
            }
        }
    }
}