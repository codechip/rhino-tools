using System;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    public class StandardMockingStrategy : AbstractMockingStrategy
    {
        #region StandardMockingStrategy()

        public StandardMockingStrategy(IAutoMockingRepository autoMock) : base(autoMock)
        {
        }

        #endregion

        public override object Create(CreationContext context, Type type)
        {
            return MockFactory.GenerateStrictMock(new []{type});
        }
    }
}