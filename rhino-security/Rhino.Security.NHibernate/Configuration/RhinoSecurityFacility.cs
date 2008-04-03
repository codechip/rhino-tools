using Castle.Core;
using Castle.MicroKernel.Registration;
using Rhino.Security.Engine.Services;
using Rhino.Security.Framework;
using Rhino.Security.Framework.Builders;

namespace Rhino.Security.Configuration
{
  /// <summary>
  /// pendent
  /// </summary>
  /// <typeparam name="TIEntityType"></typeparam>
  /// <typeparam name="TIUsersGroup"></typeparam>
  /// <typeparam name="TIEntitiesGroup"></typeparam>
  /// <typeparam name="TIOperation"></typeparam>
  /// <typeparam name="TIEntityReference"></typeparam>
  /// <typeparam name="TIPermission"></typeparam>
  public abstract class RhinoSecurityFacility<
    TIEntityType,
    TIUsersGroup,
    TIEntitiesGroup,
    TIOperation,
    TIEntityReference,
    TIPermission> : AbstractSecurityFacility
    where TIEntityType : class, IEntityType, new()
    where TIUsersGroup : class, IUsersGroup, new()
    where TIEntitiesGroup : class, IEntitiesGroup, new()
    where TIOperation : class, IOperation, new()
    where TIEntityReference : class, IEntityReference, new()
    where TIPermission : class, IPermission, new()
  {

    /// <summary>TODO: Documentation</summary>
    protected override void RegisterSecurityModelFactory()
    {
      Kernel.Register(
        Component.For<ISecurityModelFactory>()
          .ImplementedBy<GenericSecurityModelFactory<
          TIEntityType,
          TIUsersGroup,
          TIEntitiesGroup,
          TIOperation,
          TIEntityReference,
          TIPermission>>());
    }

    /// <summary>TODO: Documentation</summary>
    protected override void RegisterDefaultServices()
    {
      Kernel.Register(
        Component.For<AddCachingInterceptor>(),
        Component.For<IAuthorizationService>()
          .ImplementedBy<RhinoAuthorizationService<TIPermission>>()
          .Interceptors(new InterceptorReference(typeof(AddCachingInterceptor))).Anywhere,
        Component.For<IAuthorizationRepository>()
          .ImplementedBy<RhinoAuthorizationRepository
            <TIEntityType, TIUsersGroup, TIEntitiesGroup, TIOperation, TIEntityReference, TIPermission>>()
          .Interceptors(new InterceptorReference(typeof(AddCachingInterceptor))).Anywhere,
        Component.For<IPermissionsBuilderService>()
          .ImplementedBy<RhinoPermissionsBuilderService<TIPermission>>()
          .Interceptors(new InterceptorReference(typeof(AddCachingInterceptor))).Anywhere,
        Component.For<IPermissionsService>()
          .ImplementedBy<RhinoPermissionsService<TIPermission>>()
          .Interceptors(new InterceptorReference(typeof(AddCachingInterceptor))).Anywhere
        );
    }
  }
}