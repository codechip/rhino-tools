using System;
using Rhino.Security.Framework;

namespace Rhino.Security.Framework
{
  /// <summary>
  /// Represent a permission on the system, allow (or denying) 
  /// [operation] for [someone] on [something]
  /// </summary>
  public interface IPermission : IGloballyIdentifiable
  {
    /// <summary>
    /// Gets or sets the operation this permission applies to
    /// </summary>
    /// <value>The operation.</value>
    IOperation Operation { get; set; }

    /// <summary>
    /// Gets or sets the entity security key this permission belongs to
    /// </summary>
    /// <value>The entity security key.</value>
    Guid? EntitySecurityKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IPermission"/> is allowing 
    /// or denying the operation.
    /// </summary>
    /// <value><c>true</c> if allow; otherwise, <c>false</c>.</value>
    bool Allow { get; set; }

    /// <summary>
    /// Gets or sets the user this permission belongs to.
    /// </summary>
    /// <value>The user.</value>
    IUser User { get; set; }

    /// <summary>
    /// Gets or sets the users group this permission belongs to
    /// </summary>
    /// <value>The users group.</value>
    IUsersGroup UsersGroup { get; set; }

    /// <summary>
    /// Gets or sets the entities group this permission belongs to
    /// </summary>
    /// <value>The entities group.</value>
    IEntitiesGroup EntitiesGroup { get; set; }

    /// <summary>
    /// Gets or sets the level of this permission
    /// </summary>
    /// <value>The level.</value>
    int Level { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    /// <value>The type of the entity.</value>
    string EntityTypeName { get; set; }

    /// <summary>
    /// Sets the type of the entity.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <remarks>
    /// This uses the weak assembly name to protect us from versioning issues
    /// </remarks>
    void SetEntityType(Type type);
  }
}