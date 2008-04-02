using Iesi.Collections.Generic;
using Rhino.Security.Framework;
using Rhino.Security.Model;

namespace Rhino.Security.NH
{
  /// <summary>
  /// A named group for users, which we 
  /// can define operations on.
  /// </summary>
  public class UsersGroup : NamedEntity<UsersGroup>, IUsersGroup
  {
    private IUsersGroup parent;
    private ISet<IUsersGroup> directChildren = new HashedSet<IUsersGroup>();
    private ISet<IUsersGroup> allChildren = new HashedSet<IUsersGroup>();
    private ISet<IUsersGroup> allParent = new HashedSet<IUsersGroup>();
    private ISet<IUser> users = new HashedSet<IUser>();

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>The users.</value>
    public virtual ISet<IUser> Users
    {
      get { return users; }
      set { users = value; }
    }

    /// <summary>
    /// Gets or sets the parent of this group
    /// </summary>
    /// <value>The parent.</value>
    public virtual IUsersGroup Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    /// <summary>
    /// Gets or sets the direct children of this group (nested one level)
    /// </summary>
    /// <value>The directChildren.</value>
    public virtual ISet<IUsersGroup> DirectChildren
    {
      get { return directChildren; }
      set { directChildren = value; }
    }


    /// <summary>
    /// Gets or sets all children of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    public virtual ISet<IUsersGroup> AllChildren
    {
      get { return allChildren; }
      set { allChildren = value; }
    }


    /// <summary>
    /// Gets or sets all parent of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    public virtual ISet<IUsersGroup> AllParents
    {
      get { return allParent; }
      set { allParent = value; }
    }
  }
}