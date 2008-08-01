// Copyright (c) 2005 - 2008 Ayende Rahien (ayende@ayende.com)
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
namespace Rhino.Queues.Storage.Disk
{
	using System.Collections.Generic;
	using System.IO;

	public class DiskMessageStorageFactory : IStorageFactory
	{
		private readonly string basePath;
		private readonly bool purgeOnStartup;

		#region IStorageFactory Members

		public DiskMessageStorageFactory(string basePath, bool purgeOnStartup)
		{
			this.basePath = basePath;
			this.purgeOnStartup = purgeOnStartup;
		}

		public IMessageStorage ForOutgoingMessages(HashSet<string> endpoints)
		{
			var cleanedupEndpoints = GetCleanedupEndpoints(endpoints);
			EnsureDirectoryExists(cleanedupEndpoints.Values);
			return new DiskMessageStorage(basePath, cleanedupEndpoints);
		}

		public IMessageStorage ForIncomingMessages(HashSet<string> endpoints)
		{
			var cleanedupEndpoints = GetCleanedupEndpoints(endpoints);
			EnsureDirectoryExists(cleanedupEndpoints.Values);
			return new DiskMessageStorage(basePath, cleanedupEndpoints);
		}

		#endregion

		private void EnsureDirectoryExists(IEnumerable<string> cleanedupEndpoints)
		{
			foreach (var endpoint in cleanedupEndpoints)
			{
				var path = Path.Combine(basePath, endpoint);
				if (purgeOnStartup)
				{
					if (Directory.Exists(path))
						Directory.Delete(path, true);
				}
				if (Directory.Exists(path) == false)
					Directory.CreateDirectory(path);
			}
		}

		private static IDictionary<string, string> GetCleanedupEndpoints(IEnumerable<string> endpoints)
		{
			var cleanedupEndpoints = new Dictionary<string, string>();
			foreach (var endpoint in endpoints)
				cleanedupEndpoints.Add(endpoint, CleanEndpointName(endpoint));
			return cleanedupEndpoints;
		}

		private static string CleanEndpointName(string endpoint)
		{
			var copy = endpoint;
			foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
			{
				copy = copy.Replace(invalidFileNameChar, '_');
			}
			foreach (var invalidFileNameChar in Path.GetInvalidPathChars())
			{
				copy = copy.Replace(invalidFileNameChar, '_');
			}
			return copy;
		}
	}
}