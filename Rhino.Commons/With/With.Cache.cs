using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rhino.Commons
{
	public static partial class With
	{
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static class Caching
		{
			public static object cachingKey = new object();
			public static object cachingRegionKey = new object();
			public static object forceCacheRefreshKey = new object();

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static bool ShouldForceCacheRefresh
			{
				get
				{
					return true.Equals(Local.Data[forceCacheRefreshKey]);
				}
			}
			
			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static bool Enabled
			{
				get
				{
					return true.Equals(Local.Data[cachingKey]);
				}
			}

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static string CurrentCacheRegion
			{
				get
				{
					return (string)Local.Data[cachingKey];
				}
			}

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static void ClearQueryCacheRegion(string region)
			{
				UnitOfWork.NHibernateSessionFactory.EvictQueries(region);
			}
		}

		public static IDisposable ForceCacheRefresh()
		{
			Local.Data[Caching.forceCacheRefreshKey] = true;
			return new DisposableAction(delegate
			{
				Local.Data[Caching.forceCacheRefreshKey] = null;
			});
		}
		
		public static IDisposable TemporaryQueryCache()
		{
			string regionId = Guid.NewGuid().ToString();
			IDisposable cache = QueryCache(regionId);
			return new DisposableAction(delegate
			{
				cache.Dispose();
				Caching.ClearQueryCacheRegion(regionId);
			});
		}
		
		public static IDisposable QueryCache()
		{
			return QueryCache(null);
		}

		public static IDisposable QueryCache(string region)
		{
			Local.Data[Caching.cachingKey] = true;
			Local.Data[Caching.cachingRegionKey] = region;
			return new DisposableAction(delegate
			{
				Local.Data[Caching.cachingKey] = null;
				Local.Data[Caching.cachingRegionKey] = null;
			});
		}
	}
}
