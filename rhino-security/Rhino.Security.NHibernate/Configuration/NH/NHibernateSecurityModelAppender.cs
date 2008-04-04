using Rhino.Security.Configuration.NH;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Dynamically adds the Rhino.Security mappings to the NHibernate configuration.</summary>
  public class NHibernateSecurityModelAppender : INHibernateConfigurationModifier
  {
    private readonly SecurityModelMappingBuilder securityModelMappingBuilder;
    private readonly INHibernateSecurityConfiguration configuration;

    /// <summary>Create a new <see cref="NHibernateSecurityModelAppender"/></summary>
    /// <param name="configuration"></param>
    /// <param name="builder"></param>
    public NHibernateSecurityModelAppender(INHibernateSecurityConfiguration configuration, SecurityModelMappingBuilder builder)
    {
      securityModelMappingBuilder = builder;
      this.configuration = configuration;
    }

    /// <summary>Adds the mapping document to the NHibernate configuration.</summary>
    /// <param name="cfg"></param>
    public void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
    {
      cfg.AddDocument(securityModelMappingBuilder.Build());
    }
  }
}