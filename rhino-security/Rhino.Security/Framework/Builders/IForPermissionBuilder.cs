using Rhino.Security.Framework;

namespace Rhino.Security.Framework.Builders
{
  /// <summary>
  /// Define who this permission is for
  /// </summary>
  public interface IForPermissionBuilder
  {
    /// <summary>
    /// Set the user that this permission is built for
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns></returns>
    IOnPermissionBuilder For(IUser user);

    /// <summary>
    /// Set the users group that this permission is built for
    /// </summary>
    /// <param name="group">The group.</param>
    /// <returns></returns>
    IOnPermissionBuilder For(IUsersGroup group);

    /// <summary>
    /// Set the users group that this permission is built for
    /// </summary>
    /// <param name="usersGroupName">Name of the users group.</param>
    /// <returns></returns>
    IOnPermissionBuilder For(string usersGroupName);
  }
}