using System;

namespace Rhino.Commons.Exceptions
{
	[global::System.Serializable]
	public class PerformancePolicyViolationException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public PerformancePolicyViolationException() { }
		public PerformancePolicyViolationException(string message) : base(message) { }
		public PerformancePolicyViolationException(string message, Exception inner) : base(message, inner) { }
		protected PerformancePolicyViolationException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}