using Castle.ActiveRecord;
using Iesi.Collections.Generic;
using NHibernate.Mapping;
using Rhino.Security.Framework;

namespace Rhino.Security.AR
{
  /// <summary>
  /// A named group for users, which we 
  /// can define operations on.
  /// </summary>
  [ActiveRecord]
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
    [HasAndBelongsToMany(MapType=typeof(IUser),
      Table = "UsersToUsersGroups",
      ColumnKey = "GroupId",
      ColumnRef = "UserId"
      )]
    public virtual ISet<IUser> Users
    {
      get { return users; }
      set { users = value; }
    }

    /// <summary>
    /// Gets or sets the parent of this group
    /// </summary>
    /// <value>The parent.</value>
    [BelongsTo(Type=typeof(UsersGroup))]
    public virtual IUsersGroup Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    /// <summary>
    /// Gets or sets the direct children of this group (nested one level)
    /// </summary>
    /// <value>The directChildren.</value>
    [HasMany(typeof(UsersGroup),Inverse = true)]
    public virtual ISet<IUsersGroup> DirectChildren
    {
      get { return directChildren; }
      set { directChildren = value; }
    }


    /// <summary>
    /// Gets or sets all children of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    [HasAndBelongsToMany(
      typeof(UsersGroup),
      Table = "UsersGroupsHierarchy",
      ColumnKey = "ParentGroup",
      ColumnRef = "ChildGroup",
      Inverse = true
      )]
    public virtual ISet<IUsersGroup> AllChildren
    {
      get { return allChildren; }
      set { allChildren = value; }
    }


    /// <summary>
    /// Gets or sets all parent of this users group, at all nesting levels
    /// </summary>
    /// <value>All children.</value>
    [HasAndBelongsToMany(
      typeof(UsersGroup),
      Table = "UsersGroupsHierarchy",
      ColumnRef = "ParentGroup",
      ColumnKey = "ChildGroup"
      )]
    public virtual ISet<IUsersGroup> AllParents
    {
      get { return allParent; }
      set { allParent = value; }
    }
  }
}