using System;
using Rhino.Security.Framework;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// A reference to an entity in the domain
  /// </summary>
  public interface IEntityReference : IGloballyIdentifiable
  {
    /// <summary>
    /// Gets or sets the entity security key.
    /// </summary>
    /// <value>The entity security key.</value>
    Guid EntitySecurityKey { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity this instance
    /// reference
    /// </summary>
    /// <value>The type.</value>
    IEntityType Type { get; set; }
  }
}