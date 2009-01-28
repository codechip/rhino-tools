using System;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus
{
    public class Endpoint
    {
        public Uri Uri { get; set; }

        public Endpoint ForSubQueue(SubQueue subQueue)
        {
            return new Endpoint
            {
                Uri = new Uri(Uri + ";" + subQueue)
            };
        }

        [Obsolete("a",true)]
        public new string ToString()
        {
            return string.Format("Uri: {0}", Uri);
        }
    }
}