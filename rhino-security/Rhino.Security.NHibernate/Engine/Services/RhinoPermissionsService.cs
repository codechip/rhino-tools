using System;
using System.Collections.Generic;
using Rhino.Commons;
using NHibernate.Criterion;
using Rhino.Security.Framework;

namespace Rhino.Security.Engine.Services
{


  /// <summary>
  /// Allow to retrieve and remove permissions
  /// on users, user groups, entities groups and entities.
  /// </summary>
  public class RhinoPermissionsService<TPermission> : IPermissionsService
  {
    private readonly IAuthorizationRepository authorizationRepository;
    private readonly IRepository<TPermission> permissionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RhinoPermissionsService{TPermission}"/> class.
    /// </summary>
    /// <param name="authorizationRepository">The authorization editing service.</param>
    /// <param name="permissionRepository">The permission repository.</param>
    public RhinoPermissionsService(IAuthorizationRepository authorizationRepository,
                  IRepository<TPermission> permissionRepository)
    {
      this.authorizationRepository = authorizationRepository;
      this.permissionRepository = permissionRepository;
    }

    #region IPermissionsService Members

    /// <summary>
    /// Gets the permissions for the specified user
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns></returns>
    public IPermission[] GetPermissionsFor(IUser user)
    {
      IUsersGroup[] groups = authorizationRepository.GetAssociatedUsersGroupFor(user);

      DetachedCriteria criteria = DetachedCriteria.For<TPermission>()
        .Add(Expression.Eq("User", user) || Expression.In("UsersGroup", groups));

      return FindResults(criteria);
    }


    /// <summary>
    /// Gets the permissions for the specified etntity
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public IPermission[] GetPermissionsFor(IUser user, string operationName)
    {
      IUsersGroup[] groups = authorizationRepository.GetAssociatedUsersGroupFor(user);
      string[] operationNames = SecurityStringHelper.GetHierarchicalOperationNames(operationName);
      DetachedCriteria criteria = DetachedCriteria.For<TPermission>()
        .Add(Expression.Eq("User", user) || Expression.In("UsersGroup", groups))
        .CreateAlias("Operation", "op")
        .Add(Expression.In("op.Name", operationNames));

      return FindResults(criteria);
    }

    /// <summary>
    /// Gets the permissions for the specified user and entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="user">The user.</param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public IPermission[] GetPermissionsFor<TEntity>(IUser user, TEntity entity) where TEntity : class
    {
      Guid key = Security.ExtractKey(entity);
      IEntitiesGroup[] entitiesGroups = authorizationRepository.GetAssociatedEntitiesGroupsFor(entity);
      IUsersGroup[] usersGroups = authorizationRepository.GetAssociatedUsersGroupFor(user);

      DetachedCriteria criteria = DetachedCriteria.For<TPermission>()
        .Add(Expression.Eq("User", user) || Expression.In("UsersGroup", usersGroups))
        .Add(Expression.Eq("EntitySecurityKey", key) || Expression.In("EntitiesGroup", entitiesGroups));

      return FindResults(criteria);
    }


    /// <summary>
    /// Gets the permissions for the specified etntity
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="user">The user.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public IPermission[] GetPermissionsFor<TEntity>(IUser user, TEntity entity, string operationName) where TEntity : class
    {
      Guid key = Security.ExtractKey(entity);
      string[] operationNames = SecurityStringHelper.GetHierarchicalOperationNames(operationName);
      IEntitiesGroup[] entitiesGroups = authorizationRepository.GetAssociatedEntitiesGroupsFor(entity);
      IUsersGroup[] usersGroups = authorizationRepository.GetAssociatedUsersGroupFor(user);

      AbstractCriterion onCriteria =
          (Expression.Eq("EntitySecurityKey", key) || Expression.In("EntitiesGroup", entitiesGroups)) ||
          (Expression.IsNull("EntitiesGroup") && Expression.IsNull("EntitySecurityKey"));
      DetachedCriteria criteria = DetachedCriteria.For<TPermission>()
      .Add(Expression.Eq("User", user) || Expression.In("UsersGroup", usersGroups))
      .Add(onCriteria)
     .CreateAlias("Operation", "op")
      .Add(Expression.In("op.Name", operationNames));

      return FindResults(criteria);
    }

    /// <summary>
    /// Gets the permissions for the specified etntity
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public IPermission[] GetPermissionsFor<TEntity>(TEntity entity) where TEntity : class
    {
      if (entity is IUser)// the combpiler will direct IUser instance to here, annoying
        return GetPermissionsFor((IUser)entity);

      Guid key = Security.ExtractKey(entity);
      IEntitiesGroup[] groups = authorizationRepository.GetAssociatedEntitiesGroupsFor(entity);
      DetachedCriteria criteria = DetachedCriteria.For<TPermission>()
        .Add(Expression.Eq("EntitySecurityKey", key) || Expression.In("EntitiesGroup", groups));

      return FindResults(criteria);
    }


    #endregion

    private IPermission[] FindResults(DetachedCriteria criteria)
    {
      ICollection<TPermission> permissions = permissionRepository.FindAll(criteria,
        Order.Desc("Level"), Order.Asc("Allow"));
      return Collection.ToArray<IPermission>(permissions);
    }
  }
}