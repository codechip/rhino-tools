namespace Rhino.Security
{
    using System;

    /// <summary>Thrown when an invalid entity is passed to the security system</summary>
    [global::System.Serializable]
    public class InvalidEntityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidEntityException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InvalidEntityException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}