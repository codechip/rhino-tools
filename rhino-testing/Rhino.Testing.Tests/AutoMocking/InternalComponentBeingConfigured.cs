namespace Rhino.Testing.Tests.AutoMocking
{
    internal class InternalComponentBeingConfigured
    {
        public IReallyCoolService ReallyCoolService;
        public ICollectionOfServices Services;

        internal InternalComponentBeingConfigured(IReallyCoolService reallyCoolService, ICollectionOfServices services)
        {
            ReallyCoolService = reallyCoolService;
            Services = services;
        }

        public void RunDispose()
        {
            Services.SomethingToDispose.Dispose();
        }
    }
}
