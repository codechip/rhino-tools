using System;

namespace Rhino.DHT
{
    public class ReplicationValue
    {
        public ReplicationAction Action { get; set; }
        public string Key { get; set; }
        public ValueVersion Version { get; set; }
        public ValueVersion[] ParentVersions { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public byte[] Bytes { get; set; }
    }
}