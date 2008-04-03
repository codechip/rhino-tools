using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Rhino.Commons;
using Rhino.Security.Engine;
using Rhino.Security.Framework;

namespace Rhino.Security.Engine.Services
{
  /// <summary>
  /// Allows to edit the security information of the 
  /// system
  /// </summary>
  public class RhinoAuthorizationRepository<
    TIEntityType,
    TIUsersGroup,
    TIEntitiesGroup,
    TIOperation,
    TIEntityReference,
    TIPermission> : IAuthorizationRepository
    where TIEntityType : class, IEntityType, new()
    where TIUsersGroup : class, IUsersGroup, new()
    where TIEntitiesGroup : class, IEntitiesGroup, new()
    where TIOperation : class, IOperation, new()
    where TIEntityReference : class, IEntityReference, new()
    where TIPermission : class, IPermission, new()
  {
    /// <summary>TODO: Documentation</summary>
    protected readonly ISecurityModelFactory modelFactory;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIEntitiesGroup> entitiesGroupRepository;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIEntityReference> entityReferenceRepository;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIEntityType> entityTypesRepository;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIOperation> operationsRepository;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIPermission> permissionsRepository;
    /// <summary>TODO: Documentation</summary>
    protected readonly IRepository<TIUsersGroup> usersGroupRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RhinoAuthorizationRepository{TIEntityType,TIUsersGroup,TIEntitiesGroup,TIOperation,TIEntityReference,TIPermission}"/> class.
    /// </summary>
    /// <param name="modelFactory">The factory to create instances of the <see cref="Rhino.Security.Model"/></param>
    /// <param name="usersGroupRepository">The users group repository.</param>
    /// <param name="entitiesGroupRepository">The entities group repository.</param>
    /// <param name="entityReferenceRepository">The entity reference repository.</param>
    /// <param name="entityTypesRepository">The entity types repository.</param>
    /// <param name="operationsRepository">The operations repository.</param>
    /// <param name="permissionsRepository">The permissionss repository.</param>
    public RhinoAuthorizationRepository(ISecurityModelFactory modelFactory,
                                   IRepository<TIUsersGroup> usersGroupRepository,
                                   IRepository<TIEntitiesGroup> entitiesGroupRepository,
                                   IRepository<TIEntityReference> entityReferenceRepository,
                                   IRepository<TIEntityType> entityTypesRepository,
                                   IRepository<TIOperation> operationsRepository,
                                   IRepository<TIPermission> permissionsRepository)
    {
      this.modelFactory = modelFactory;
      this.usersGroupRepository = usersGroupRepository;
      this.permissionsRepository = permissionsRepository;
      this.operationsRepository = operationsRepository;
      this.entityTypesRepository = entityTypesRepository;
      this.entityReferenceRepository = entityReferenceRepository;
      this.entitiesGroupRepository = entitiesGroupRepository;
    }

    #region IAuthorizationRepository Members

    /// <summary>
    /// Creates a new users group.
    /// </summary>
    /// <param name="name">The name of the new group.</param>
    public virtual IUsersGroup CreateUsersGroup(string name)
    {
      IUsersGroup ug = modelFactory.CreateUsersGroup(name);
      //if (validator.IsValid(ug) == false)
      //{
      //  ErrorSummary summary = validator.GetErrorSummary(ug);
      //  throw new ValidationException(summary);
      //}
      usersGroupRepository.Save((TIUsersGroup)ug);
      return ug;
    }

    /// <summary>
    /// Creates the users group as a child of <paramref name="parentGroupName"/>.
    /// </summary>
    /// <param name="parentGroupName">Name of the parent group.</param>
    /// <param name="usersGroupName">Name of the users group.</param>
    /// <returns></returns>
    public virtual IUsersGroup CreateChildUserGroupOf(string parentGroupName, string usersGroupName)
    {
      IUsersGroup parent = GetUsersGroupByName(parentGroupName);
      Guard.Against<ArgumentException>(parent == null,
                                       "Parent users group '" + parentGroupName + "' does not exists");

      IUsersGroup group = CreateUsersGroup(usersGroupName);
      group.Parent = parent;
      group.AllParents.AddAll(parent.AllParents);
      group.AllParents.Add(parent);
      parent.DirectChildren.Add(group);
      parent.AllChildren.Add(group);
      return group;
    }

    /// <summary>
    /// temporary string
    /// </summary>
    /// <param name="usersGroupName">Name of the users group.</param>
    public virtual void RemoveUsersGroup(string usersGroupName)
    {
      IUsersGroup group = GetUsersGroupByName(usersGroupName);
      if (group == null)
        return;

      Guard.Against(group.DirectChildren.Count != 0, "Cannot remove users group '"+usersGroupName+"' because is has child groups. Remove those groups and try again.");

      DetachedCriteria permissionsToRemove = DetachedCriteria.For<TIPermission>()
        .Add(Expression.Eq("UsersGroup", group));

      permissionsRepository.DeleteAll(permissionsToRemove);
      // we have to do this in order to ensure that we play
      // nicely with the second level cache and collection removals
      if (group.Parent!=null)
      {
        group.Parent.DirectChildren.Remove(group);
      }
      foreach (IUsersGroup parent in group.AllParents)
      {
        parent.AllChildren.Remove(group);
      }
      group.AllParents.Clear();
      group.Users.Clear();

      usersGroupRepository.Delete((TIUsersGroup)group);
    }


    /// <summary>
    /// Removes the specified entities group.
    /// Will also delete all permissions that are associated with this group.
    /// </summary>
    /// <param name="entitesGroupName">Name of the entites group.</param>
    public virtual void RemoveEntitiesGroup(string entitesGroupName)
    {
      IEntitiesGroup group = GetEntitiesGroupByName(entitesGroupName);
      if(group==null)
        return;

      DetachedCriteria permissionsToRemove = DetachedCriteria.For<TIPermission>()
        .Add(Expression.Eq("EntitiesGroup", group));

      permissionsRepository.DeleteAll(permissionsToRemove);

      group.Entities.Clear();

      entitiesGroupRepository.Delete((TIEntitiesGroup)group);
    }


    /// <summary>
    /// Removes the specified operation.
    /// Will also delete all permissions for this operation
    /// </summary>
    /// <param name="operationName">The operation N ame.</param>
    public virtual void RemoveOperation(string operationName)
    {
      IOperation operation = GetOperationByName(operationName);
      if(operation==null)
        return;

      Guard.Against(operation.Children.Count != 0, "Cannot remove operation '"+operationName+"' because it has child operations. Remove those operations and try again.");

      DetachedCriteria permissionsToRemove = DetachedCriteria.For<TIPermission>()
        .Add(Expression.Eq("Operation", operation));

      permissionsRepository.DeleteAll(permissionsToRemove);
            
      // so we can play safely with the 2nd level cache & collections
      if(operation.Parent!=null)
      {
        operation.Parent.Children.Remove(operation);
      }

      operationsRepository.Delete((TIOperation)operation);
    }

    /// <summary>
    /// Gets the ancestry association of a user with the named users group.
    /// This allows to track how a user is associated to a group through 
    /// their ancestry.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="usersGroupName">Name of the users group.</param>
    /// <returns></returns>
    public virtual IUsersGroup[] GetAncestryAssociation(IUser user, string usersGroupName)
    {
      TIUsersGroup desiredGroup = (TIUsersGroup) GetUsersGroupByName(usersGroupName);
      ICollection<TIUsersGroup> directGroups =
        usersGroupRepository.FindAll(GetDirectUserGroupsCriteria(user));
      if (directGroups.Contains(desiredGroup))
      {
        return new IUsersGroup[] {desiredGroup};
      }
      // as a nice benefit, this does an eager load of all the groups in the hierarchy
      // in an efficient way, so we don't have SELECT N + 1 here, nor do we need
      // to load the Users collection (which may be very large) to check if we are associated
      // directly or not
      IUsersGroup[] associatedGroups = GetAssociatedUsersGroupFor(user);
      if (Array.IndexOf(associatedGroups, desiredGroup) == -1)
      {
        return new IUsersGroup[0];
      }
      // now we need to find out the path to it
      List<TIUsersGroup> shortest = new List<TIUsersGroup>();
      foreach (TIUsersGroup usersGroup in associatedGroups)
      {
        List<TIUsersGroup> path = new List<TIUsersGroup>();
        TIUsersGroup current = usersGroup;
        while (current != null && current != desiredGroup)
        {
          path.Add(current);
          current = (TIUsersGroup)current.Parent;
        }
        if (current != null)
          path.Add(current);
        // Valid paths are those that are contains the desired group
        // and start in one of the groups that are directly associated
        // with the user
        if (path.Contains(desiredGroup) && directGroups.Contains(path[0]))
        {
          shortest = Min(shortest, path);
        }
      }
      return shortest.ToArray();
    }

    /// <summary>
    /// Creates a new entities group.
    /// </summary>
    /// <param name="name">The name of the new group.</param>
    public virtual IEntitiesGroup CreateEntitiesGroup(string name)
    {
      TIEntitiesGroup eg = (TIEntitiesGroup) modelFactory.CreateEntitiesGroup(name);
      //if (validator.IsValid(eg) == false)
      //{
      //  ErrorSummary summary = validator.GetErrorSummary(eg);
      //  throw new ValidationException(summary);
      //}
      entitiesGroupRepository.Save(eg);
      return eg;
    }

    /// <summary>
    /// Gets the associated users group for the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    public virtual IUsersGroup[] GetAssociatedUsersGroupFor(IUser user)
    {
      DetachedCriteria directGroupsCriteria = GetDirectUserGroupsCriteria(user)
        .SetProjection(Projections.Id());

      DetachedCriteria allGroupsCriteria = DetachedCriteria.For<TIUsersGroup>()
        .CreateAlias("Users", "user", JoinType.LeftOuterJoin)
        .CreateAlias("AllChildren", "child", JoinType.LeftOuterJoin)
        .Add(
        Subqueries.PropertyIn("child.id", directGroupsCriteria) ||
        Expression.Eq("user.id", user.SecurityInfo.Identifier));

      ICollection<TIUsersGroup> usersGroups =
        usersGroupRepository.FindAll(allGroupsCriteria, Order.Asc("Name"));
      return Collection.ToArray<IUsersGroup>(usersGroups);
    }


    /// <summary>
    /// Gets the users group by its name
    /// </summary>
    /// <param name="groupName">Name of the group.</param>
    public virtual IUsersGroup GetUsersGroupByName(string groupName)
    {
      return usersGroupRepository.FindOne(Expression.Eq("Name", groupName));
    }

    /// <summary>
    /// Gets the entities group by its groupName
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    public virtual IEntitiesGroup GetEntitiesGroupByName(string groupName)
    {
      return entitiesGroupRepository.FindOne(Expression.Eq("Name", groupName));
    }


    /// <summary>
    /// Gets the groups the specified entity is associated with
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public virtual IEntitiesGroup[] GetAssociatedEntitiesGroupsFor<TEntity>(TEntity entity) where TEntity : class
    {
      Guid key = Security.ExtractKey(entity);
      DetachedCriteria criteria = DetachedCriteria.For<TIEntitiesGroup>()
        .CreateAlias("Entities", "e")
        .Add(Expression.Eq("e.EntitySecurityKey", key));
      ICollection<TIEntitiesGroup> entitiesGroups = entitiesGroupRepository.FindAll(criteria);
      return Collection.ToArray<IEntitiesGroup>(entitiesGroups);
    }

    /// <summary>
    /// Associates the entity with a group with the specified name
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <param name="groupName">Name of the group.</param>
    public virtual void AssociateEntityWith<TEntity>(TEntity entity, string groupName) where TEntity : class
    {
      IEntitiesGroup entitiesGroup = GetEntitiesGroupByName(groupName);
      Guard.Against<ArgumentException>(entitiesGroup == null, "There is no entities group named: " + groupName);

      AssociateEntityWith(entity, entitiesGroup);
    }

    /// <summary>
    /// Associates the entity with the specified group
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <param name="entitiesGroup">The entities group.</param>
    public void AssociateEntityWith<TEntity>(TEntity entity, IEntitiesGroup entitiesGroup) where TEntity : class
    {
      Guid key = Security.ExtractKey(entity);

      IEntityReference reference = GetOrCreateEntityReference<TEntity>(key);
      entitiesGroup.Entities.Add(reference);
    }


    /// <summary>
    /// Associates the user with a group with the specified name
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="groupName">Name of the group.</param>
    public virtual void AssociateUserWith(IUser user, string groupName)
    {
      IUsersGroup group = GetUsersGroupByName(groupName);
      Guard.Against(group == null, "There is no users group named: " + groupName);

      AssociateUserWith(user, group);
    }

    /// <summary>
    /// Associates the user with a group with the specified name
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="group">The group.</param>
    public void AssociateUserWith(IUser user, IUsersGroup group)
    {
      group.Users.Add(user);
    }

    /// <summary>
    /// Creates the operation with the given name
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public virtual IOperation CreateOperation(string operationName)
    {
      Guard.Against<ArgumentException>(string.IsNullOrEmpty(operationName), "operationName must have a value");
      Guard.Against<ArgumentException>(operationName[0] != '/', "Operation names must start with '/'");

      IOperation op = modelFactory.CreateOperation(operationName);

      //if (validator.IsValid(op) == false)
      //  throw new ValidationException(validator.GetErrorSummary(op));

      string parentOperationName = SecurityStringHelper.GetParentOperationName(operationName);
      if (parentOperationName != string.Empty) //we haven't got to the root
      {
        IOperation parentOperation = GetOperationByName(parentOperationName);
        if (parentOperation == null)
          parentOperation = CreateOperation(parentOperationName);

        op.Parent = parentOperation;
        parentOperation.Children.Add(op);
      }

      operationsRepository.Save((TIOperation)op);
      return op;
    }

    /// <summary>
    /// Gets the operation by the specified name
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns></returns>
    public virtual IOperation GetOperationByName(string operationName)
    {
      return operationsRepository.FindOne(Expression.Eq("Name", operationName));
    }

    /// <summary>
    /// Removes the user from the specified group
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="usersGroupName">Name of the users group.</param>
    public void DetachUserFromGroup(IUser user, string usersGroupName)
    {
      IUsersGroup group = GetUsersGroupByName(usersGroupName);
      Guard.Against(group == null, "There is no users group named: " + usersGroupName);

      group.Users.Remove(user);
    }

    /// <summary>
    /// Removes the entities from the specified group
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity.</param>
    /// <param name="entitiesGroupName">Name of the entities group.</param>
    public void DetachEntityFromGroup<TEntity>(TEntity entity, string entitiesGroupName) where TEntity : class
    {
      IEntitiesGroup entitiesGroup = GetEntitiesGroupByName(entitiesGroupName);
      Guard.Against<ArgumentException>(entitiesGroup == null,
                                       "There is no entities group named: " + entitiesGroupName);
      Guid key = Security.ExtractKey(entity);

      IEntityReference reference = GetOrCreateEntityReference<TEntity>(key);
      entitiesGroup.Entities.Remove(reference);
    }


    /// <summary>
    /// Removes the user from rhino security.
    /// This does NOT delete the user itself, merely reset all the
    /// information that rhino security knows about it.
    /// It also allows it to be removed by external API without violating
    /// FK constraints
    /// </summary>
    /// <param name="user">The user.</param>
    public void RemoveUser(IUser user)
    {
      ICollection<TIUsersGroup> groups = usersGroupRepository.FindAll(GetDirectUserGroupsCriteria(user));
      foreach (TIUsersGroup group in groups)
      {
        group.Users.Remove(user);
      }

      DetachedCriteria permissionsToRemove = DetachedCriteria.For<TIPermission>()
        .Add(Expression.Eq("User", user));
      permissionsRepository.DeleteAll(permissionsToRemove);
    }


    /// <summary>
    /// Removes the specified permission.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public void RemovePermission(IPermission permission)
    {
      permissionsRepository.Delete((TIPermission)permission);
    }

    #endregion

    private static List<TIUsersGroup> Min(List<TIUsersGroup> first, List<TIUsersGroup> second)
    {
      if (first.Count == 0)
        return second;
      if (first.Count <= second.Count)
        return first;
      return second;
    }

    private DetachedCriteria GetDirectUserGroupsCriteria(IUser user)
    {
      return DetachedCriteria.For<TIUsersGroup>()
        .CreateAlias("Users", "user")
        .Add(Expression.Eq("user.id", user.SecurityInfo.Identifier));
    }

    private IEntityReference GetOrCreateEntityReference<TEntity>(Guid key)
    {
      TIEntityReference reference = entityReferenceRepository.FindOne(Expression.Eq("EntitySecurityKey", key));
      if (reference == null)
      {
        reference = (TIEntityReference) modelFactory.CreateEntityReference(key);
        reference.Type = GetOrCreateEntityType<TEntity>();
        entityReferenceRepository.Save(reference);
      }
      return reference;
    }

    private IEntityType GetOrCreateEntityType<TEntity>()
    {
      TIEntityType entityType = entityTypesRepository.FindOne(
        Expression.Eq("Name", typeof (TEntity).FullName)
        );
      if (entityType == null)
      {
        entityType = (TIEntityType) modelFactory.CreateEntityType(typeof(TEntity));
        entityTypesRepository.Save(entityType);
      }
      return entityType;
    }
  }
}