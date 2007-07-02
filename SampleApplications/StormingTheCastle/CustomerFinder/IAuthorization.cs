namespace CustomerFinder
{
    public interface IAuthorization
    {
        bool IsAllowed(Customer customer, string operation);
    }
}