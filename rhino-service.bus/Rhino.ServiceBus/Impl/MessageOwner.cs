using System;
using System.Reflection;

namespace Rhino.ServiceBus.Impl
{
    public class MessageOwner
    {
        public Assembly Assembly;
        public Uri Endpoint;

        public bool IsOwner(object msg)
        {
            return msg.GetType().Assembly == Assembly;
        }

    }
}