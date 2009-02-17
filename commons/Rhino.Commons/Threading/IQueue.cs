namespace Rhino.Commons
{
    public interface IQueue<T>
    {
        void Enqueue(T o);
        T Dequeue();
        bool IsEmpty { get; }
    }
}