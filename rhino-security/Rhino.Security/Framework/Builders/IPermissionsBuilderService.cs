using Rhino.Security.Framework;
using Rhino.Security.Framework.Builders;

namespace Rhino.Security.Framework.Builders
{
  /// <summary>
  /// Allow to define permissions using a fluent interface
  /// </summary>
  public interface IPermissionsBuilderService
  {
    /// <summary>
    /// Allow permission for the specified operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    IForPermissionBuilder Allow(string operationName);

    /// <summary>
    /// Deny permission for the specified operation 
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    IForPermissionBuilder Deny(string operationName);

    /// <summary>
    /// Allow permission for the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    IForPermissionBuilder Allow(IOperation operation);

    /// <summary>
    /// Deny permission for the specified operation 
    /// </summary>
    /// <param name="operation">The operation.</param>
    IForPermissionBuilder Deny(IOperation operation);
  }
}