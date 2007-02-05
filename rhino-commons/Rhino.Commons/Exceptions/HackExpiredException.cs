using System;

namespace Rhino.Commons.Exceptions
{
	[global::System.Serializable]
	public class HackExpiredException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public HackExpiredException() { }
		public HackExpiredException(string message) : base(message) { }
		public HackExpiredException(string message, Exception inner) : base(message, inner) { }
		protected HackExpiredException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}