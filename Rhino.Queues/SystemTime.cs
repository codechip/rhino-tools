using System;

namespace Rhino.Queues
{
	public static class SystemTime
	{
		public static Func<DateTime> Now = () => DateTime.Now;
	}
}