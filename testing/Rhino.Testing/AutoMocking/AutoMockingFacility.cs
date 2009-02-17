using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    public class AutoMockingFacility : IFacility
    {
        private IAutoMockingRepository _autoMock;

        public AutoMockingFacility(IAutoMockingRepository autoMock)
        {
            _autoMock = autoMock;
        }

        #region IFacility Members

        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Resolver.AddSubResolver(new AutoMockingDependencyResolver(_autoMock));
            kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
        }

        public void Terminate()
        {
        }

        #endregion

        private void OnComponentModelCreated(ComponentModel model)
        {
            _autoMock.Mark(model.Service).NotMocked();
        }
    }
}