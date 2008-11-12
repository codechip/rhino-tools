using System;

namespace Rhino.Commons
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class IsCandidateForRepositoryAttribute : Attribute
    {
    }
}
