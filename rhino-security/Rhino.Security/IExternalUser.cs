namespace Rhino.Security
{
    /// <summary>
    /// This is an extension interface that allows to integrate with user
    /// entities that are not stored in the database.
    /// </summary>
    /// <remarks>
    /// Implementors of this interface must supply a default constructor
    /// </remarks>
    public interface IExternalUser : IUser
    {
        /// <summary>
        /// Loads the specified user from the specified identifier
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        void Load(string identifier);
    }
}