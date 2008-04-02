using Iesi.Collections.Generic;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// Represent an operation in the system
  /// </summary>
  public interface IOperation : INamedEntity
  {

    /// <summary>
    /// Gets or sets the comment for this operation
    /// </summary>
    /// <value>The comment.</value>
    string Comment { get; set; }

    /// <summary>
    /// Gets or sets the parent of this operation
    /// </summary>
    /// <value>The parent.</value>
    IOperation Parent { get; set; }

    /// <summary>
    /// Gets or sets the children of this operation
    /// </summary>
    /// <value>The children.</value>
    ISet<IOperation> Children { get; set; }
  }
}