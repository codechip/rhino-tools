using System;

namespace Rhino.Commons
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class IsCandidateForRepositoryAttribute : Attribute
    {
        public static bool IsCandidate(Type possibleCandidate, Type owner)
        {
            return possibleCandidate.GetCustomAttributes(typeof(IsCandidateForRepositoryAttribute), false).Length == 1;
        }
    }
}
