namespace Rhino.Security.Tests
{
    using System.Security.Principal;

    public class WindowsIdentityUserAdapter : IExternalUser
    {
        private WindowsIdentity identity;

        public WindowsIdentityUserAdapter()
        {
        }

        public WindowsIdentityUserAdapter(WindowsIdentity identity)
        {
            this.identity = identity;
        }

        #region IExternalUser Members

        /// <summary>
        /// Gets or sets the security info for this user
        /// </summary>
        /// <value>The security info.</value>
        public SecurityInfo SecurityInfo
        {
            get { return new SecurityInfo(identity.Name, identity.User.Value); }
        }

        /// <summary>
        /// Loads the specified user from the specified identifier
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public void Load(string identifier)
        {
            identity = new WindowsIdentity(identifier);
        }

        #endregion
    }
}