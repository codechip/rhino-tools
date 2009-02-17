namespace Rhino.Testing.Tests.AutoMocking
{
    public class ComponentBeingConfiguredWithInternalCtor
    {
        public IReallyCoolService ReallyCoolService;
        public ICollectionOfServices Services;

        internal ComponentBeingConfiguredWithInternalCtor(IReallyCoolService reallyCoolService, ICollectionOfServices services)
        {
            ReallyCoolService = reallyCoolService;
            Services = services;
        }

        public ComponentBeingConfiguredWithInternalCtor(IReallyCoolService reallyCoolService)
        {
            ReallyCoolService = reallyCoolService;
        }

        public void RunDispose()
        {
            Services.SomethingToDispose.Dispose();
        }
    }
}
