using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;

namespace Rhino.Commons.Test.NHibernate
{
    class MockCacheProvider : ICacheProvider 
    {
        class MockCache : ICache
        {
            public object Get(object key)
            {
                //noop
                return null;
            }

            public void Put(object key, object value)
            {
                //noop
            }

            public void Remove(object key)
            {
                //noop
            }

            public void Clear()
            {
                //noop
            }

            public void Destroy()
            {
                //noop
            }

            public void Lock(object key)
            {
                //noop
            }

            public void Unlock(object key)
            {
                //noop
            }

            public long NextTimestamp()
            {
                return DateTime.Now.Ticks;
            }

            public int Timeout
            {
                get { return 0; }
            }

            public string RegionName
            {
                get { return "MyRegion"; }
            }
        }

        public static bool IsStopped = true;
        public static bool IsInitialized = false;

        private readonly static Maintenance maintenance = new Maintenance();

        public static bool IsRunning
        {
            get
            {
                return maintenance.IsRunning;
            }
        }
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            return new MockCache();
        }

        public long NextTimestamp()
        {
            return DateTime.Now.Ticks;
        }

        public void Start(IDictionary<string, string> properties)
        {
            IsInitialized = true;
            IsStopped = false;
            maintenance.Start();
        }

        public void Stop()
        {
            IsStopped = true;
        }

        class Maintenance
        {
            public readonly Thread thread;

            public Maintenance()
            {
                thread = new Thread(MaintenanceThread);
            }
            public void Start()
            {
                thread.Start();
            }

            public bool IsRunning = false;
            private void MaintenanceThread()
            {
                do
                {
                    IsRunning = true;
                    Thread.Sleep(100);
                } while (IsStopped == false);
                IsRunning = false;
            }
        }
    }
}
