using System;
using System.Runtime.Serialization;

namespace Rhino.ServiceBus.Exceptions
{
    [Serializable]
    public class SubscriptionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SubscriptionException()
        {
        }

        public SubscriptionException(string message) : base(message)
        {
        }

        public SubscriptionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SubscriptionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}