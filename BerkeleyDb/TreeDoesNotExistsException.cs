using System;
using System.Runtime.Serialization;

namespace BerkeleyDb
{
	[Serializable]
	public class TreeDoesNotExistsException : Exception
	{
		public TreeDoesNotExistsException(string message)
			: base(message)
		{
		}

		public TreeDoesNotExistsException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected TreeDoesNotExistsException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}