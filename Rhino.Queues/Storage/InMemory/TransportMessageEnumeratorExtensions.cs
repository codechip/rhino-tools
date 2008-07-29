namespace Rhino.Queues.Storage.InMemory
{
	using System.Collections.Generic;
	using Impl;

	public static class TransportMessageEnumeratorExtensions
	{
		public static void RemoveCurrent(this IEnumerable<TransportMessage> enumerable)
		{
			var self = (QueuePackage.ReverseEnumerable) enumerable;
			self.RemoveCurrent();
		}
	}
}