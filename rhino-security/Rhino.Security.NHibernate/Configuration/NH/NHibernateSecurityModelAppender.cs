using Rhino.Security.Configuration.NH;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Dynamically adds the Rhino.Security mappings to the NHibernate configuration.</summary>
  public class NHibernateSecurityModelAppender : INHibernateConfigurationModifier
  {
    private readonly SecurityModelMappingBuilder securityModelMappingBuilder;

    /// <summary>Create a new <see cref="NHibernateSecurityModelAppender"/></summary>
    /// <param name="builder"></param>
    public NHibernateSecurityModelAppender(SecurityModelMappingBuilder builder)
    {
      securityModelMappingBuilder = builder;
    }

    /// <summary>Adds the mapping document to the NHibernate configuration.</summary>
    /// <param name="cfg"></param>
    public void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
    {
      cfg.AddDocument(securityModelMappingBuilder.Build());
    }
  }
}