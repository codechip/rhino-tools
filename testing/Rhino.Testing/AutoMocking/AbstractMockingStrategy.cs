using System;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public abstract class AbstractMockingStrategy : IMockingStrategy
    {
        private IAutoMockingRepository _autoMock;

        public AbstractMockingStrategy(IAutoMockingRepository autoMock)
        {
            _autoMock = autoMock;
        }

        public IAutoMockingRepository AutoMock
        {
            get { return _autoMock; }
        }

        public IMockFactory MockFactory
        {
          get { return _autoMock.MockFactory; }
        }

        public MockRepository Mocks
        {
            get { return _autoMock.MockRepository; }
        }

        #region IMockingStrategy Members

        public abstract object Create(CreationContext context, Type type);

        #endregion
    }
}