using Castle.Components.Validator;
using Rhino.Commons;
using Rhino.Security.Framework;

namespace Rhino.Security.Engine.Services
{
  public class ARValidatingAuthorizationRepository<TIEntityType, TIUsersGroup, TIEntitiesGroup, TIOperation, TIEntityReference, TIPermission> 
    :  RhinoAuthorizationRepository< TIEntityType, TIUsersGroup, TIEntitiesGroup, TIOperation, TIEntityReference, TIPermission> 
      where TIEntityType : class, IEntityType, new()
      where TIUsersGroup : class, IUsersGroup, new()
      where TIEntitiesGroup : class, IEntitiesGroup, new()
      where TIOperation : class, IOperation, new()
      where TIEntityReference : class, IEntityReference, new()
      where TIPermission : class, IPermission, new()
  {
    private readonly ValidatorRunner validator;

    public ARValidatingAuthorizationRepository(ISecurityModelFactory modelFactory,
                                               IRepository<TIUsersGroup> usersGroupRepository,
                                               IRepository<TIEntitiesGroup> entitiesGroupRepository,
                                               IRepository<TIEntityReference> entityReferenceRepository,
                                               IRepository<TIEntityType> entityTypesRepository,
                                               IRepository<TIOperation> operationsRepository,
                                               IRepository<TIPermission> permissionsRepository,
                                               ValidatorRunner validator)
      : base(modelFactory, usersGroupRepository, entitiesGroupRepository, entityReferenceRepository, entityTypesRepository, operationsRepository, permissionsRepository)
    {
      this.validator = validator;
    }

    public override IEntitiesGroup CreateEntitiesGroup(string name)
    {
      TIEntitiesGroup eg = (TIEntitiesGroup)modelFactory.CreateEntitiesGroup(name);
      if (validator.IsValid(eg) == false)
      {
        ErrorSummary summary = validator.GetErrorSummary(eg);
        throw new ValidationException(summary);
      }
      entitiesGroupRepository.Save(eg);
      return eg;
    }

    public override IUsersGroup CreateUsersGroup(string name)
    {
      IUsersGroup ug = modelFactory.CreateUsersGroup(name);
      if (validator.IsValid(ug) == false)
      {
        ErrorSummary summary = validator.GetErrorSummary(ug);
        throw new ValidationException(summary);
      }
      usersGroupRepository.Save((TIUsersGroup)ug);
      return ug;
    }
  }
}