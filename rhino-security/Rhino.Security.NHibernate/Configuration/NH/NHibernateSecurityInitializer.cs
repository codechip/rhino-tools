using NHibernate;
using Rhino.Commons;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Implements <see cref="INHibernateInitializationAware"/>.</summary>
  public class NHibernateSecurityInitializer : INHibernateInitializationAware
  {
    private readonly INHibernateConfigurationModifier securityModelAppender;

    /// <summary>Creates a new <see cref="NHibernateSecurityInitializer"/></summary>
    /// <param name="securityModelAppender"></param>
    public NHibernateSecurityInitializer(INHibernateConfigurationModifier securityModelAppender)
    {
      this.securityModelAppender = securityModelAppender;
    }

    /// <summary>After initial configuration</summary>
    /// <param name="cfg"></param>
    public virtual void Configured(NHibernate.Cfg.Configuration cfg)
    {
      AppendSecurityModel(cfg);
    }

    /// <summary>Adds the security model to the NHibernate configuration</summary>
    /// <param name="cfg"></param>
    protected virtual void AppendSecurityModel(NHibernate.Cfg.Configuration cfg)
    {
      securityModelAppender.AlterConfiguration(cfg);
    }

    /// <summary>After initialization</summary>
    /// <param name="cfg"></param>
    /// <param name="sessionFactory"></param>
    public virtual void Initialized(NHibernate.Cfg.Configuration cfg, ISessionFactory sessionFactory)
    {
    }
  }
}