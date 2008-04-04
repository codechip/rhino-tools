namespace Rhino.Security.Configuration.NH
{
  /// <summary>Service which is called after the initial nhibernate configuration but before
  /// the session factory is built.  implementors are allowed to dynamically alter the configuration
  /// and mappings of nhibernate.</summary>
  public interface INHibernateConfigurationModifier
  {
    /// <summary>Alters the NHibernate configuration.</summary>
    /// <param name="cfg">The configuration.</param>
    void AlterConfiguration(NHibernate.Cfg.Configuration cfg);
  }
}