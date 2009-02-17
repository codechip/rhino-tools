namespace Rhino.Testing.Tests.AutoMocking
{
    public class ComponentBeingConfigured
    {
        public IReallyCoolService ReallyCoolService;
        public ICollectionOfServices Services;

        public ComponentBeingConfigured(IReallyCoolService reallyCoolService, ICollectionOfServices services)
        {
            ReallyCoolService = reallyCoolService;
            Services = services;
        }

        public ComponentBeingConfigured(IReallyCoolService reallyCoolService)
        {
            ReallyCoolService = reallyCoolService;
        }

        public void RunDispose()
        {
            Services.SomethingToDispose.Dispose();
        }
    }
}
