namespace Rhino.Security.Framework
{
  /// <summary>
  /// An entity with a name
  /// </summary>
  public interface INamedEntity : IGloballyIdentifiable
  {
    /// <summary>
    /// Gets or sets the name of this entity.
    /// </summary>
    /// <value>The name.</value>
    /// <remarks>
    /// The name can be set only on creation, and is not changed
    /// afterward.
    /// </remarks>
    string Name { get; set; }
  }
}