namespace Rhino.Testing.Tests.AutoMocking
{
    public class ComponentBeingConfiguredWithPrivateCtor
    {
        public IReallyCoolService ReallyCoolService;
        public ICollectionOfServices Services;

        private ComponentBeingConfiguredWithPrivateCtor(IReallyCoolService reallyCoolService, ICollectionOfServices services)
        {
            ReallyCoolService = reallyCoolService;
            Services = services;
        }

        public ComponentBeingConfiguredWithPrivateCtor(IReallyCoolService reallyCoolService) : this(reallyCoolService, null)
        {
        }

        public void RunDispose()
        {
            Services.SomethingToDispose.Dispose();
        }
    }
}
