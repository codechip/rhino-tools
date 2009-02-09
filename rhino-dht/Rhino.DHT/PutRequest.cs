using System;

namespace Rhino.DHT
{
    public class PutRequest
    {
        public string Key { get; set; }
        public ValueVersion[] ParentVersions { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public byte[] Bytes { get; set; }
        public bool OptimisticConcurrency { get; set; }
    }
}