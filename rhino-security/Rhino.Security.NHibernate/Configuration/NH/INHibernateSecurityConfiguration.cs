using System;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Defines the configuration for Rhino Security using NHibernate</summary>
  public interface INHibernateSecurityConfiguration
  {
    /// <summary>The type of table structure to use, prefixed or schema</summary>
    SecurityTableStructure TableStructure { get; set; }

    /// <summary>The concrente user class that implements <see cref="Rhino.Security.Framework.IUser"/>.</summary>
    Type UserType { get; set; }
  }
}