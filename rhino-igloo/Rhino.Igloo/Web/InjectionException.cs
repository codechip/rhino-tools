using System;

namespace Rhino.Igloo
{
	/// <summary>
	/// Thrown when there is a problem during injection 
	/// </summary>
    [global::System.Serializable]
    public class InjectionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

		/// <summary>
		/// Initializes a new instance of the <see cref="InjectionException"/> class.
		/// </summary>
        public InjectionException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InjectionException"/> class.
		/// </summary>
		/// <param myName="message">The message.</param>
        public InjectionException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InjectionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
        public InjectionException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InjectionException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected InjectionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}