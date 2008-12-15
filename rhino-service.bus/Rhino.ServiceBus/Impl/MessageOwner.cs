using System;
using System.Reflection;

namespace Rhino.ServiceBus.Impl
{
    public class MessageOwner
    {
        public Assembly Assembly;
        public Uri Endpoint;

        public bool IsOwner(Type msg)
        {
            return msg.Assembly == Assembly;
        }

    }
}