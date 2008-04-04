using System;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Default implementation of configuration</summary>
  public class NHibernateSecurityConfiguration : INHibernateSecurityConfiguration
  {
    private Type userType;
    private SecurityTableStructure tableStructure;

    /// <summary>BUG: Microkernel needs this to fullfill the dependencies
    /// even though we use an ExternalActivator with an instance.</summary>
    public NHibernateSecurityConfiguration()
      : this(SecurityTableStructure.Prefix, null)
    {
    }
    /// <summary>Creates a new configuration</summary>
    /// <param name="tableStructure"></param>
    /// <param name="userType"></param>
    public NHibernateSecurityConfiguration(SecurityTableStructure tableStructure, Type userType)
    {
      this.tableStructure = tableStructure;
      this.userType = userType;
    }

    /// <summary>The table structure mode</summary>
    public SecurityTableStructure TableStructure
    {
      get { return tableStructure; }
      set { tableStructure = value; }
    }

    /// <summary>The concrete user type</summary>
    public Type UserType
    {
      get { return userType; }
      set { userType = value; }
    }
  }
}