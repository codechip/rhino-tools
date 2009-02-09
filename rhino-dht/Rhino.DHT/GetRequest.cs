using System;

namespace Rhino.DHT
{
    public class GetRequest
    {
        public string Key{ get; set;}
        public ValueVersion SpecifiedVersion { get; set; }
    }
}