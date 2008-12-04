namespace Advance.IoC.HierarchicalContainers
{
    public class DefaultUsersService : IUsersService
    {
        private readonly ISender sender;

        public DefaultUsersService(ISender sender)
        {
            this.sender = sender;
        }

        public ISender Sender
        {
            get { return sender; }
        }

        public void ResetPassword()
        {
            
        }
    }
}