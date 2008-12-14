using System;

namespace Rhino.ServiceBus.Msmq
{
    public class QueueDescriptor
    {
        public bool IsLocal;
        public Uri Uri;
        public string QueuePath;
        public string FullQueuePath;
    }
}