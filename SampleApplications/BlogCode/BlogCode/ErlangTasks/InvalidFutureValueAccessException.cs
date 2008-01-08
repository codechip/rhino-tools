namespace pipelines
{
    using System;

    [global::System.Serializable]
    public class InvalidFutureValueAccessException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidFutureValueAccessException() { }
        public InvalidFutureValueAccessException(string message) : base(message) { }
        public InvalidFutureValueAccessException(string message, Exception inner) : base(message, inner) { }
        protected InvalidFutureValueAccessException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}