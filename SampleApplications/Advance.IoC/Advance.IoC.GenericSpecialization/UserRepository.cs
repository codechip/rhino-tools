namespace Advance.IoC.GenericSpecialization
{
    public class UserRepository : IRepository<User>
    {
        public User Get()
        {
            return new User();
        }
    }
}