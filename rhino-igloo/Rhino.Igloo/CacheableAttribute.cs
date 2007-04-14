using System;

namespace Rhino.Igloo
{
    /// <summary>
    /// Indicate that this is a cachable entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CacheableAttribute : Attribute
    {
        
    }
}