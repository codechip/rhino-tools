using System;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Internal;
using Castle.Components.Validator;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Rhino.Security.AR;
using Rhino.Security.AR.Configuration;
using Rhino.Security.Configuration;
using Rhino.Security.Engine.Services;

namespace Rhino.Security.Configuration
{
  public class ARSecurityFacility
    : RhinoSecurityFacility<EntityType, UsersGroup, EntitiesGroup, Operation, EntityReference, Permission>
  {
    private readonly Type userType;
    private readonly SecurityTableStructure tableStructure;


    public ARSecurityFacility(Type userType)
      : this(userType, SecurityTableStructure.Schema)
    {
      this.userType = userType;
    }


    public ARSecurityFacility(Type userType, SecurityTableStructure tableStructure)
    {
      this.tableStructure = tableStructure;
      this.userType = userType;
    }

    public virtual Type UserType
    {
      get { return userType; }
    }

    public virtual SecurityTableStructure TableStructure
    {
      get { return tableStructure; }
    }

    protected override void RegisterDefaultServices()
    {
      PrepareForActiveRecordInitialization();
      Kernel.Register(
        Component.For<IAuthorizationRepository>()
          .ImplementedBy<ARValidatingAuthorizationRepository
          <EntityType, UsersGroup, EntitiesGroup, Operation, EntityReference, Permission>>()
          .Interceptors(new InterceptorReference(typeof(AddCachingInterceptor))).Anywhere
        );
      base.RegisterDefaultServices();
    }

    /// <summary>
    /// Prepares to change all internal reference in the security system
    /// from IUser to the user implementation of the project
    /// </summary>
    protected virtual void PrepareForActiveRecordInitialization()
    {
      ModelsDelegate validated = null;
      validated = delegate(ActiveRecordModelCollection models, IConfigurationSource source)
        {
          ActiveRecordStarter.ModelsValidated -= validated;
          foreach (ActiveRecordModel model in models)
          {
            if (model.Type.Assembly != typeof(Permission).Assembly)
              continue;
            model.Accept(new AddCachingVisitor());
            model.Accept(new ReplaceUserVisitor(UserType));
            model.Accept(new ChangeSchemaVisitor(TableStructure));
          }
        };
      ActiveRecordStarter.ModelsValidated += validated;
    }

    protected override void RegisterValidationServices()
    {
      if (Kernel.HasComponent(typeof(IValidatorRegistry)) == false)
      {
        Kernel.Register(
          Component.For<IValidatorRegistry>()
            .ImplementedBy<CachedValidationRegistry>()
          );
      }

      if (Kernel.HasComponent(typeof(ValidatorRunner)) == false)
      {
        Kernel.Register(
          Component.For<ValidatorRunner>()
          );
      }
    }
  }
}