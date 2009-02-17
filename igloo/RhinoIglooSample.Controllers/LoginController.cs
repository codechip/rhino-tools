namespace RhinoIglooSample.Controllers
{
    using System;
    using Model;
    using NHibernate.Criterion;
    using Rhino.Commons;
    using Rhino.Igloo;

    public class LoginController : AbstractController
    {
        private string name;
        private string password;

        [Inject]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Inject]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public LoginController(IContext context, IRepository<User> repository) : base(context, repository)
        {
        }

        public bool Authenticate()
        {
            User user = GetUserByNameAndPassword(Name, Password);
            Scope.Session["username"] = user.Name;
            if(user.PasswordExpiresAt < DateTime.Today)
            {
                Context.Redirect("ChangePassword.aspx",false);
            }
            return user != null;
        }

        protected override void AssertUserIsAuthorized()
        {
            //well, anyone can go to the login page.
        }
    }
}
