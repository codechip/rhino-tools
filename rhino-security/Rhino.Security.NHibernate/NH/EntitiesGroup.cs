using Iesi.Collections.Generic;
using Rhino.Security.Framework;
using Rhino.Security.Model;

namespace Rhino.Security.NH
{
  /// <summary>
  /// A grouping of entities, with some business meaning.
  /// </summary>
  public class EntitiesGroup : NamedEntity<EntitiesGroup>, IEntitiesGroup
  {
    private ISet<IEntityReference> entities = new HashedSet<IEntityReference>();

    /// <summary>
    /// Gets or sets the entities security keys beloging to this entities group
    /// </summary>
    /// <value>The entities security keys.</value>
    public virtual ISet<IEntityReference> Entities
    {
      get { return entities; }
      set { entities = value; }
    }
  }
}