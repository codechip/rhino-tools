namespace Advance.IoC.HierarchicalContainers
{
    public interface IUsersService
    {
        ISender Sender { get;  }
        void ResetPassword();
    }
}