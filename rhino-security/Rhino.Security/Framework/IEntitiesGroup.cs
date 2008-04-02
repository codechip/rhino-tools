using Iesi.Collections.Generic;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// A grouping of entities, with some business meaning.
  /// </summary>
  public interface IEntitiesGroup : INamedEntity
  {
    /// <summary>
    /// Gets or sets the entities security keys beloging to this entities group
    /// </summary>
    /// <value>The entities security keys.</value>
    ISet<IEntityReference> Entities { get; set; }
  }
}