namespace Advance.IoC.GenericSpecialization
{
    public interface IRepository<T>
    {
        T Get();
    }
}