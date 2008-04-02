using System;
using Rhino.Commons;
using Rhino.Security.Framework;
using Rhino.Security.Framework.Builders;

namespace Rhino.Security.Engine.Services
{
  /// <summary>
  /// Allow to define permissions using a fluent interface
  /// </summary>
  public class RhinoPermissionsBuilderService<TPermission> : IPermissionsBuilderService
  {
    private readonly IRepository<TPermission> permissionRepository;
    private readonly IAuthorizationRepository authorizationRepository;
    private readonly ISecurityModelFactory securityModelFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="RhinoPermissionsBuilderService{TPermission}"/> class.
    /// </summary>
    /// <param name="securityModelFactory">The factory to create instances of the Security model objects.</param>
    /// <param name="permissionRepository">The permission repository.</param>
    /// <param name="authorizationRepository">The authorization editing service.</param>
    public RhinoPermissionsBuilderService(ISecurityModelFactory securityModelFactory, IRepository<TPermission> permissionRepository, IAuthorizationRepository authorizationRepository)
    {
      this.securityModelFactory = securityModelFactory;
      this.permissionRepository = permissionRepository;
      this.authorizationRepository = authorizationRepository;
    }

    /// <summary>
    /// Builds a permission
    /// </summary>
    public class FluentPermissionBuilder : IPermissionBuilder, IForPermissionBuilder, IOnPermissionBuilder,
                                           ILevelPermissionBuilder
    {
      private readonly IPermission permission;
      private readonly RhinoPermissionsBuilderService<TPermission> permissionBuilderService;

      /// <summary>
      /// Initializes a new instance of the <see cref="FluentPermissionBuilder"/> class.
      /// </summary>
      /// <param name="permissionBuilderService">The permission service.</param>
      /// <param name="newPermission">An instance of the new permission that will be configured from the builder service.</param>
      /// <param name="allow">if set to <c>true</c> create an allow permission.</param>
      /// <param name="operation">The operation.</param>
      public FluentPermissionBuilder(RhinoPermissionsBuilderService<TPermission> permissionBuilderService, IPermission newPermission, bool allow, IOperation operation)
      {
        this.permissionBuilderService = permissionBuilderService;
        permission = newPermission;
        permission.Allow = allow;
        permission.Operation = operation;
      }

      /// <summary>
      /// Save the created permission
      /// </summary>
      public IPermission Save()
      {
        permissionBuilderService.Save(permission);
        return permission;
      }

      /// <summary>
      /// Set the user that this permission is built for
      /// </summary>
      /// <param name="user">The user.</param>
      /// <returns></returns>
      public IOnPermissionBuilder For(IUser user)
      {
        permission.User = user;
        return this;
      }


      /// <summary>
      /// Set the users group that this permission is built for
      /// </summary>
      /// <param name="usersGroupName">Name of the users group.</param>
      /// <returns></returns>
      public IOnPermissionBuilder For(string usersGroupName)
      {
        IUsersGroup usersGroup = permissionBuilderService
          .authorizationRepository
          .GetUsersGroupByName(usersGroupName);

        Guard.Against<ArgumentException>(usersGroup == null, "There is not users group named: " + usersGroup);

        return For(usersGroup);
      }

      /// <summary>
      /// Set the users group that this permission is built for
      /// </summary>
      /// <param name="usersGroup">The users group.</param>
      /// <returns></returns>
      public IOnPermissionBuilder For(IUsersGroup usersGroup)
      {
        permission.UsersGroup = usersGroup;

        return this;
      }

      /// <summary>
      /// Set the entity this permission is built for
      /// </summary>
      /// <typeparam name="TEntity">The type of the entity.</typeparam>
      /// <param name="entity">The account.</param>
      /// <returns></returns>
      public ILevelPermissionBuilder On<TEntity>(TEntity entity) where TEntity : class
      {
        permission.SetEntityType(typeof (TEntity));
        permission.EntitySecurityKey = Security.ExtractKey(entity);
        return this;
      }


      /// <summary>
      /// Set the entity group this permission is built for
      /// </summary>
      /// <param name="entitiesGroupName">Name of the entities group.</param>
      /// <returns></returns>
      public ILevelPermissionBuilder On(string entitiesGroupName)
      {
        IEntitiesGroup entitiesGroup = 
          permissionBuilderService
            .authorizationRepository
            .GetEntitiesGroupByName(entitiesGroupName);
        Guard.Against<ArgumentException>(entitiesGroup == null,
                                         "There is no entities group named: " + entitiesGroupName);
        return On(entitiesGroup);
      }

      /// <summary>
      /// Set the entity group this permission is built for
      /// </summary>
      /// <param name="entitiesGroup">The entities group.</param>
      /// <returns></returns>
      public ILevelPermissionBuilder On(IEntitiesGroup entitiesGroup)
      {
        permission.EntitiesGroup = entitiesGroup;
        return this;
      }


      /// <summary>
      /// Set this permission to be application to everything
      /// </summary>
      /// <returns></returns>
      public ILevelPermissionBuilder OnEverything()
      {
        return this;
      }

      /// <summary>
      /// Define the level of this permission
      /// </summary>
      /// <param name="level">The level.</param>
      /// <returns></returns>
      public IPermissionBuilder Level(int level)
      {
        permission.Level = level;
        return this;
      }


      /// <summary>
      /// Define the default level;
      /// </summary>
      /// <returns></returns>
      public IPermissionBuilder DefaultLevel()
      {
        return Level(1);
      }
    }

    /// <summary>
    /// Saves the specified permission
    /// </summary>
    /// <param name="permission">The permission.</param>
    public void Save(IPermission permission)
    {
      permissionRepository.Save((TPermission)permission);
    }

    /// <summary>
    /// Allow permission for the specified operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public IForPermissionBuilder Allow(string operationName)
    {
      IOperation operation = authorizationRepository.GetOperationByName(operationName);
      Guard.Against<ArgumentException>(operation == null, "There is no operation named: " + operationName);
      return Allow(operation);
    }

    /// <summary>
    /// Deny permission for the specified operation 
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public IForPermissionBuilder Deny(string operationName)
    {
      IOperation operation = authorizationRepository.GetOperationByName(operationName);
      Guard.Against<ArgumentException>(operation == null, "There is no operation named: " + operationName);
      return Deny(operation);
    }


    /// <summary>
    /// Allow permission for the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns></returns>
    public IForPermissionBuilder Allow(IOperation operation)
    {
      return new FluentPermissionBuilder(this, securityModelFactory.CreatePermission(), true, operation);
    }

    /// <summary>
    /// Deny permission for the specified operation
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns></returns>
    public IForPermissionBuilder Deny(IOperation operation)
    {
      return new FluentPermissionBuilder(this, securityModelFactory.CreatePermission(), false, operation);
    }
  }
}