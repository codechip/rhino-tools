namespace BDSLiB.QuoteGeneration
{
    public class RequirementsInformation
    {
        public int UserCount;
        public string[] RequestedModules;

        public RequirementsInformation(int userCount, params string[] requestedModules)
        {
            UserCount = userCount;
            RequestedModules = requestedModules;
        }
    }
}