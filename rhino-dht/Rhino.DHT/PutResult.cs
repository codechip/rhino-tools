namespace Rhino.DHT
{
    public class PutResult
    {
        public int Version { get; set; }
        public bool ConflictExists { get; set; }
    }
}