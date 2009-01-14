namespace Rhino.ServiceBus.Internal
{
    public interface IValueConvertor<T>
    {
        string ToString(T val);
        T FromString(string val);
    }
}