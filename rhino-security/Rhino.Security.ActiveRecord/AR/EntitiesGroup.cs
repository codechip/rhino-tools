using Castle.ActiveRecord;
using Iesi.Collections.Generic;
using Rhino.Security.Framework;

namespace Rhino.Security.AR
{
  /// <summary>
  /// A grouping of entities, with some business meaning.
  /// </summary>
  [ActiveRecord]
  public class EntitiesGroup : NamedEntity<EntitiesGroup>, IEntitiesGroup
  {
    private ISet<IEntityReference> entities = new HashedSet<IEntityReference>();

    /// <summary>
    /// Gets or sets the entities security keys beloging to this entities group
    /// </summary>
    /// <value>The entities security keys.</value>
    [HasAndBelongsToMany(
      typeof(EntityReference),
      Table = "EntityReferencesToEntitiesGroups",
      ColumnKey = "GroupId",
      ColumnRef = "EntityReferenceId"
      )]
    public virtual ISet<IEntityReference> Entities
    {
      get { return entities; }
      set { entities = value; }
    }
  }
}