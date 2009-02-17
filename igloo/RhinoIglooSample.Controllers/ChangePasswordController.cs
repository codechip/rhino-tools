namespace RhinoIglooSample.Controllers
{
    using System;
    using Castle.Services.Transaction;
    using Model;
    using Rhino.Commons;
    using Rhino.Igloo;

    [Transactional]
    public class ChangePasswordController : AbstractController
    {
        private string username;
        private string oldPass;
        private string newPass;
        private string newPassConfirm;

        [Inject(Scope = ScopeType.Session)]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [Inject]
        public string OldPass
        {
            get { return oldPass; }
            set { oldPass = value; }
        }

        [Inject]
        public string NewPass
        {
            get { return newPass; }
            set { newPass = value; }
        }

        [Inject]
        public string NewPassConfirm
        {
            get { return newPassConfirm; }
            set { newPassConfirm = value; }
        }

        public ChangePasswordController(IContext context, IRepository<User> repository) : base(context, repository)
        {
        }

        [Transaction]
        public virtual bool ChangePassword()
        {
            if(NewPass != NewPassConfirm)
            {
                Scope.ErrorMessage = "Password and confirm password must be identical";
                return false;
            }
            if(!SatisfyPasswordComplexity(NewPass))
            {
                Scope.ErrorMessage = "Password is not complex enough";
                return false;
            }
            User user = GetUserByNameAndPassword(Username, OldPass);
            user.Password = NewPass;
            user.PasswordExpiresAt = DateTime.Today.AddDays(7);
            users.Save(user);
            Scope.SuccessMessage = "Password changed successfully";
            return true;
        }

        private static bool SatisfyPasswordComplexity(string pass)
        {
            if(string.IsNullOrEmpty(pass))
                return false;
            return pass.Length > 5;
        }

        protected override void AssertUserIsAuthorized()
        {
            // well, anyone can use the change password to change
            // their own passwords
        }
    }
}
