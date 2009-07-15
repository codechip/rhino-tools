using System;
using System.Reflection;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public class StubbedStrategy : AbstractMockingStrategy
    {
        #region Member Data

        private StandardMockingStrategy _default;

        #endregion

        #region StubbedStrategy()

        public StubbedStrategy(IAutoMockingRepository autoMock)
            : base(autoMock)
        {
            _default = new StandardMockingStrategy(autoMock);
        }

        #endregion

        public override object Create(CreationContext context, Type type)
        {
            object target = MockFactory.GenerateStub(new []{type});
            AutoMock.AddService(type, target);
            return target;
        }
    }
}