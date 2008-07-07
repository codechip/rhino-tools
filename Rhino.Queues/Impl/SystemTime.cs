using System;

namespace Rhino.Queues.Impl
{
	public static class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.Now;
	}
}