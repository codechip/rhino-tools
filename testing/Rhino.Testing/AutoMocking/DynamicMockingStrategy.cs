using System;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    public class DynamicMockingStrategy : AbstractMockingStrategy
    {
        public DynamicMockingStrategy(IAutoMockingRepository autoMock) : base(autoMock)
        {
        }

        public override object Create(CreationContext context, Type type)
        {
          return MockFactory.GenerateDynamicMock(new []{type});
        }
    }
}