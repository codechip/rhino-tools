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
					return (string)Local.Data[cachingRegionKey];
				}
			}

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public static void ClearQueryCacheRegion(string region)
			{
				UnitOfWork.CurrentSession
                    .GetSessionImplementation()
                    .Factory
                    .EvictQueries(region);
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
			object prevCachingValueKey = Local.Data[Caching.cachingKey];
			object prevCachingRegion = Local.Data[Caching.cachingRegionKey];

			Local.Data[Caching.cachingKey] = true;
			Local.Data[Caching.cachingRegionKey] = region;
			
			return new DisposableAction(delegate
			{
				Local.Data[Caching.cachingKey] = prevCachingValueKey;
				Local.Data[Caching.cachingRegionKey] = prevCachingRegion;
			});
		}
	}
}
