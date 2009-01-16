namespace Rhino.DHT.Client
{
    public interface ICache
    {
        void Delete(string key);
        void Put(string key, string type, byte[] value);
        bool Get(string key, out CachedItem item);
    }
}