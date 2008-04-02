using Castle.ActiveRecord;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using Rhino.Security.Framework;

namespace Rhino.Security.AR
{
  /// <summary>
  /// Represent an operation in the system
  /// </summary>
  [ActiveRecord]
  public class Operation : NamedEntity<Operation>, IOperation
  {
    private string comment;
    private IOperation parent;
    private ISet<IOperation> children = new HashedSet<IOperation>();

    /// <summary>
    /// Gets or sets the comment for this operation
    /// </summary>
    /// <value>The comment.</value>
    [Property(Length = 255)]
    [ValidateLength(0, 255)]
    public virtual string Comment
    {
      get { return comment; }
      set { comment = value; }
    }

    /// <summary>
    /// Gets or sets the parent of this operation
    /// </summary>
    /// <value>The parent.</value>
    [BelongsTo(Type = typeof(Operation))]
    public virtual IOperation Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    /// <summary>
    /// Gets or sets the children of this operation
    /// </summary>
    /// <value>The children.</value>
    [HasMany(typeof(Operation), Inverse = true)]
    public virtual ISet<IOperation> Children
    {
      get { return children; }
      set { children = value; }
    }
  }
}