using System;

namespace Rhino.ServiceBus.Messages
{
    public class HeartBeat
    {
        public DateTime At { get; set; }
        public Uri From { get; set; }
    }
}
