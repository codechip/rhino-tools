namespace Rhino.DHT
{
    public class Value
    {
        public string Key { get; set; }
        public int Version { get; set; }
        public byte[] Data { get; set; }
    }
}