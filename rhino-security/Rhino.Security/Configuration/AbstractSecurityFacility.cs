using Castle.MicroKernel.Facilities;

namespace Rhino.Security.Configuration
{
  /// <summary>
  /// Setup everything necessary for Rhino Security to function
  /// </summary>
  public abstract class AbstractSecurityFacility : AbstractFacility
  {
    ///<summary>The custom initialization for the Facility.</summary>
    protected override void Init()
    {
      RegisterSecurityModelFactory();
      RegisterRepositories();
      RegisterDefaultServices();
      RegisterValidationServices();
    }

    /// <summary>
    /// TODO: Doc 
    /// </summary>
    protected virtual void RegisterSecurityModelFactory()
    {
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    protected virtual void RegisterRepositories()
    {
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    protected virtual void RegisterValidationServices()
    {
    }

    /// <summary>
    /// TODO: Doc
    /// </summary>
    protected virtual void RegisterDefaultServices()
    {
    }
  }
}