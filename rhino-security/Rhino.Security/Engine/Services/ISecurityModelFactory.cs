using System;
using Rhino.Security.Framework;

namespace Rhino.Security.Engine.Services
{
  /// <summary>A factory create concrete implementations of the security model objects.</summary>
  public interface ISecurityModelFactory
  {
    /// <summary>Creates an <see cref="IEntityType"/> and sets the <see cref="INamedEntity.Name"/> to value given the <paramref name="forEntityType"/>.</summary>
    /// <param name="forEntityType">The type of entity that will be referenced by the <see cref="IEntityType"/></param>
    /// <returns>A a new instance of <see cref="IEntityType"/> with its <see cref="INamedEntity.Name"/> set.</returns>
    IEntityType CreateEntityType(Type forEntityType);

    /// <summary>Creates a <see cref="IUsersGroup"/> with the given <paramref name="name"/>.</summary>
    /// <param name="name">The name of the new <see cref="IUsersGroup"/>.</param>
    /// <returns>An new instance of <see cref="IUsersGroup"/> with its <see cref="INamedEntity.Name"/> set.</returns>
    IUsersGroup CreateUsersGroup(string name);

    /// <summary>Creates a <see cref="IEntitiesGroup" /> with the given <paramref name="name"/>.</summary>
    /// <param name="name">The name of the new <see cref="IEntitiesGroup"/>.</param>
    /// <returns>A new instance of <see cref="IEntitiesGroup"/> with its <see cref="INamedEntity.Name"/> set.</returns>
    IEntitiesGroup CreateEntitiesGroup(string name);

    /// <summary>Creates a <see cref="IOperation"/> with the given <paramref name="name"/>.</summary>
    /// <param name="name">The name of the new <see cref="IOperation"/>.</param>
    /// <returns>A new instance of <see cref="IOperation"/> with its <see cref="INamedEntity.Name"/> set.</returns>
    IOperation CreateOperation(string name);

    /// <summary>Creates a <see cref="IEntityReference"/> with the given <paramref name="entitySecurityKey"/>.</summary>
    /// <param name="entitySecurityKey">The <see cref="IEntityReference.EntitySecurityKey"/></param>
    /// <returns>A new instance of <see cref="IEntityReference"/> with its <see cref="IEntityReference.EntitySecurityKey"/> set.</returns>
    IEntityReference CreateEntityReference(Guid entitySecurityKey);

    /// <summary>Create a new <see cref="IPermission"/></summary>
    /// <returns></returns>
    IPermission CreatePermission();
  }
}