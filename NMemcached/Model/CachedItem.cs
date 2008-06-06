using System;

namespace NMemcached.Model
{
	public class CachedItem
	{
		private byte[] buffer;

		public uint Flags;
		public string Key;
		public long Timestamp;

		/// <summary>
		/// We need this here not becuase we don't trust the cache expiry, but because
		/// we need to support flush_all ability to set a date for all items to expire at.
		/// </summary>
		public DateTime ExpiresAt;

		public byte[] Buffer
		{
			get { return buffer; }
			set
			{
				buffer = value;
				Timestamp = SystemTime.Now().Ticks;
			}
		}
	}
}