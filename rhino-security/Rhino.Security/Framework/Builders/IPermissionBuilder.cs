using Rhino.Security.Framework;

namespace Rhino.Security.Framework.Builders
{
  /// <summary>
  /// Save the created permission
  /// </summary>
  public interface IPermissionBuilder
  {
    /// <summary>
    /// Save the created permission
    /// </summary>
    IPermission Save();
  }
}