using System;
using Rhino.Security.Framework;

namespace Rhino.Security.Engine.Services
{
  /// <summary>A Generic default implementation of <see cref="ISecurityModelFactory"/>.
  /// Each concrete type must have a public default constructor in order to use this factory.</summary>
  /// <typeparam name="TIEntityType">A concrete type of <see cref="IEntityType"/>.</typeparam>
  /// <typeparam name="TIUsersGroup">A concrete type of <see cref="IUsersGroup"/>.</typeparam>
  /// <typeparam name="TIEntitiesGroup">A concrete type of <see cref="IEntitiesGroup"/>.</typeparam>
  /// <typeparam name="TIOperation">A concrete type of <see cref="IOperation"/>.</typeparam>
  /// <typeparam name="TIEntityReference">A concrete type of <see cref="IEntityReference"/>.</typeparam>
  /// <typeparam name="TIPermission">A concrete type of <see cref="IPermission"/>.</typeparam>
  public class GenericSecurityModelFactory<
    TIEntityType, 
    TIUsersGroup, 
    TIEntitiesGroup, 
    TIOperation, 
    TIEntityReference, 
    TIPermission> : ISecurityModelFactory
    where TIEntityType : class, IEntityType, new()
    where TIUsersGroup : class, IUsersGroup, new()
    where TIEntitiesGroup : class, IEntitiesGroup, new()
    where TIOperation : class, IOperation, new()
    where TIEntityReference : class, IEntityReference, new()
    where TIPermission : class, IPermission, new()
  {
    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <param name="forEntityType"></param>
    /// <returns></returns>
    public IEntityType CreateEntityType(Type forEntityType)
    {
      IEntityType entityType = new TIEntityType();
      entityType.Name = forEntityType.FullName;
      return entityType;
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IUsersGroup CreateUsersGroup(string name)
    {
      IUsersGroup usersGroup = new TIUsersGroup();
      usersGroup.Name = name;
      return usersGroup;
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEntitiesGroup CreateEntitiesGroup(string name)
    {
      IEntitiesGroup entitiesGroup = new TIEntitiesGroup();
      entitiesGroup.Name = name;
      return entitiesGroup;
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IOperation CreateOperation(string name)
    {
      IOperation operation = new TIOperation();
      operation.Name = name;
      return operation;
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <param name="entitySecurityKey"></param>
    /// <returns></returns>
    public IEntityReference CreateEntityReference(Guid entitySecurityKey)
    {
      IEntityReference entityReference = new TIEntityReference();
      entityReference.EntitySecurityKey = entitySecurityKey;
      return entityReference;
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    /// <returns></returns>
    public IPermission CreatePermission()
    {
      return new TIPermission();
    }
  }
}