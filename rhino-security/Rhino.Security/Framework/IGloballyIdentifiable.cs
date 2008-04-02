using System;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// Mark an entity with an id
  /// </summary>
  public interface IGloballyIdentifiable
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The id.</value>
    Guid Id { get; set; }
  }
}