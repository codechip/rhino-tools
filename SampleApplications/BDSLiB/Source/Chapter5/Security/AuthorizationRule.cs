namespace Chapter5.Security
{
    using System.Security.Principal;

    public abstract class AuthorizationRule
    {
        private bool? allowed;
        private readonly object entity;
        private string message;
        private readonly IPrincipal principal;

        protected AuthorizationRule(IPrincipal principal, object entity)
        {
            this.entity = entity;
            this.principal = principal;
        }

        public abstract string Operation { get; }

        protected IPrincipal Principal
        {
            get { return principal; }
        }

        protected object Entity
        {
            get { return entity; }
        }

        public string Message
        {
            get { return message; }
        }

        public bool? Allowed
        {
            get { return allowed; }
        }

        protected void Allow(string reason)
        {
            message = reason;
            allowed = true;
        }

        protected void Deny(string reason)
        {
            message = reason;
            allowed = false;
        }

        public abstract void CheckAuthorization();
    }
}