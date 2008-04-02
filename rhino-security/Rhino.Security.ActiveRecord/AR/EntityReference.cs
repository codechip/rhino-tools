using System;
using Castle.ActiveRecord;
using Rhino.Security.Framework;

namespace Rhino.Security.AR
{
  /// <summary>
  /// A reference to an entity in the domain
  /// </summary>
  [ActiveRecord]
  public class EntityReference : EqualityAndHashCodeProvider<EntityReference>, IEntityReference
  {
    private IEntityType type;
    private Guid entitySecurityKey;

    /// <summary>
    /// Gets or sets the entity security key.
    /// </summary>
    /// <value>The entity security key.</value>
    [Property(NotNull = true,Unique = true)]
    public virtual Guid EntitySecurityKey
    {
      get { return entitySecurityKey; }
      set { entitySecurityKey = value; }
    }

    /// <summary>
    /// Gets or sets the type of the entity this instance
    /// reference
    /// </summary>
    /// <value>The type.</value>
    [BelongsTo(Type = typeof(EntityType), NotNull = true)]
    public virtual IEntityType Type
    {
      get { return type; }
      set { type = value; }
    }
  }
}