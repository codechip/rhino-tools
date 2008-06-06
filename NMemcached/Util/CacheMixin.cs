using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace NMemcached.Util
{
	public class CacheMixin
	{
		protected Cache Cache = HttpRuntime.Cache;

		protected static TimeSpan NoSlidingExpiration
		{
			get { return System.Web.Caching.Cache.NoSlidingExpiration; }
		}

		protected void ClearCache()
		{
			foreach (DictionaryEntry de in Cache)
			{
				Cache.Remove(de.Key.ToString());
			}
		}
	}
}