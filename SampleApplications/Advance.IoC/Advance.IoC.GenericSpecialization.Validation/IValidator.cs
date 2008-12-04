namespace Advance.IoC.GenericSpecialization.Validation
{
    public interface IValidator<T>
    {
        string[] Validate(T instnace);
    }
}