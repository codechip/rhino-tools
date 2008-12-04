namespace Advance.IoC.GenericSpecialization
{
    public class DefaultRepository<T> : IRepository<T>
    {
        public T Get()
        {
            return default(T);
        }
    }
}