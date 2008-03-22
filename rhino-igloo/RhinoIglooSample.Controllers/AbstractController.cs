namespace RhinoIglooSample.Controllers
{
    using Model;
    using NHibernate.Criterion;
    using Rhino.Commons;
    using Rhino.Igloo;

    public abstract class AbstractController : BaseController
    {
        protected readonly IRepository<User> users;

        public override void Initialize()
        {
            AssertUserIsAuthorized();
        }

        protected abstract void AssertUserIsAuthorized();

        public AbstractController(IContext context, IRepository<User> repository) : base(context)
        {
            this.users = repository;
        }

        protected User GetUserByNameAndPassword(string name, string password)
        {
            return users.FindOne(
                Expression.Eq("Name", name), 
                Expression.Eq("Password", password)
                );
        }
    }
}