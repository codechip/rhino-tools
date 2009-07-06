using System;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
  public class PartialMockingStrategy : AbstractMockingStrategy
  {
    public PartialMockingStrategy(IAutoMockingRepository autoMock) : base(autoMock)
    {
    }

    public override object Create(CreationContext context, Type type)
    {
      return Mocks.PartialMock(type);
    }
  }
}