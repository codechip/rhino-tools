namespace Rhino.PersistentHashTable
{
    public class RemoveRequest
    {
        public string Key{ get; set;}
        public ValueVersion[] ParentVersions { get; set; }
    }
}