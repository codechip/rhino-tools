namespace Rhino.DHT
{
    public class AddValue
    {
        public string Key { get; set; }
        public int[] ParentVersions { get; set; }
        public byte[] Bytes { get; set; }
    }
}