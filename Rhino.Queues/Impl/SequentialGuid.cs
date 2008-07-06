using System;
using System.Runtime.InteropServices;

namespace Rhino.Queues.Impl
{
	public static class SequentialGuid
	{
		[DllImport("rpcrt4.dll", SetLastError = true)]
		static extern int UuidCreateSequential(out Guid guid);

		public static Guid Next()
		{
			Guid g;
			UuidCreateSequential(out g);
			return g;
		}
	}
}