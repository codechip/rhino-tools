using System.Xml;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Builds an NHibernate XML Mapping file.</summary>
  public interface INHibernateMappingBuilder
  {
    /// <summary>Builds the security model and returns the NHibernate mapping as an <see cref="XmlDocument"/>.</summary>
    /// <returns>A <see cref="XmlDocument"/> as an NHibernate mapping document.</returns>
    XmlDocument Build();
  }
}