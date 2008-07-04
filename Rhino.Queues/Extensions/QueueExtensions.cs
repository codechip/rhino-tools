using System;

namespace Rhino.Queues.Extensions
{
	public static class QueueExtensions
	{
		public static string ToQueueName(this Uri uri)
		{
			return uri.LocalPath.Substring(1);
		}
	}
}