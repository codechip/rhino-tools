using System;
using Castle.MicroKernel.Registration;
using Rhino.Commons;
using Rhino.Security.Configuration.NH;
using Rhino.Security.NH;
using Parameter=Castle.MicroKernel.Registration.Parameter;

namespace Rhino.Security.Configuration
{
  /// <summary>TODO: Documentation</summary>
  public class NHSecurityFacility : RhinoSecurityFacility<EntityType,UsersGroup,EntitiesGroup,Operation,EntityReference,Permission>
  {
    private readonly INHibernateSecurityConfiguration configuration;

    /// <summary>TODO: Documentation</summary>
    /// <param name="userType"></param>
    public NHSecurityFacility(Type userType)
      : this(userType, SecurityTableStructure.Prefix)
    {
    }

    /// <summary>TODO: Documentation</summary>
    /// <param name="userType"></param>
    /// <param name="tableStructure"></param>
    public NHSecurityFacility(Type userType, SecurityTableStructure tableStructure)
    {
      configuration = new NHibernateSecurityConfiguration(tableStructure, userType);
    }

    /// <summary>TODO: Documentation</summary>
    protected override void RegisterDefaultServices()
    {
      Kernel.Register(
        Component.For<INHibernateSecurityConfiguration>()
          .Instance(configuration),
        Component.For<SecurityModelMappingBuilder>()
          .ImplementedBy<SecurityModelMappingBuilder>(),
        Component.For<INHibernateConfigurationModifier>()
          .ImplementedBy<NHibernateSecurityModelAppender>(),
        Component.For<INHibernateInitializationAware>()
          .ImplementedBy<NHibernateSecurityInitializer>()
        );
      base.RegisterDefaultServices();
    }
  }
}