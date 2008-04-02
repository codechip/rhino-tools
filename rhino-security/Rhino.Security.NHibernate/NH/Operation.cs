using Iesi.Collections.Generic;
using Rhino.Security.Framework;
using Rhino.Security.Model;

namespace Rhino.Security.NH
{
  /// <summary>
  /// Represent an operation in the system
  /// </summary>
  public class Operation : NamedEntity<Operation>, IOperation
  {
    private string comment;
    private IOperation parent;
    private ISet<IOperation> children = new HashedSet<IOperation>();

    /// <summary>
    /// Gets or sets the comment for this operation
    /// </summary>
    /// <value>The comment.</value>
    public virtual string Comment
    {
      get { return comment; }
      set { comment = value; }
    }

    /// <summary>
    /// Gets or sets the parent of this operation
    /// </summary>
    /// <value>The parent.</value>
    public virtual IOperation Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    /// <summary>
    /// Gets or sets the children of this operation
    /// </summary>
    /// <value>The children.</value>
    public virtual ISet<IOperation> Children
    {
      get { return children; }
      set { children = value; }
    }
  }
}