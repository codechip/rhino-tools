using System;

namespace Rhino.Igloo
{
    /// <summary>
    /// Signify that the controller should not be automatically registered in the view.
    /// Usually because it needs additional configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NoAutomaticRegistrationInContainer : Attribute
    {
        
    }
}