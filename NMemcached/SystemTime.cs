using System;

namespace NMemcached
{
	public class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.Now;
	}
}