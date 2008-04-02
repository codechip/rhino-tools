using Iesi.Collections.Generic;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// A named group for users, which we 
  /// can define operations on.
  /// </summary>
  public interface IUsersGroup : INamedEntity
  {
    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>The users.</value>
    ISet<IUser> Users { get; set; }

    /// <summary>
    /// Gets or sets the parent of this group
    /// </summary>
    /// <value>The parent.</value>
    IUsersGroup Parent { get; set; }

    /// <summary>
    /// Gets or sets the direct children of this group (nested one level)
    /// </summary>
    /// <value>The directChildren.</value>
    ISet<IUsersGroup> DirectChildren { get; set; }

    /// <summary>
    /// Gets or sets all children of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    ISet<IUsersGroup> AllChildren { get; set; }

    /// <summary>
    /// Gets or sets all parent of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    ISet<IUsersGroup> AllParents { get; set; }
  }
}