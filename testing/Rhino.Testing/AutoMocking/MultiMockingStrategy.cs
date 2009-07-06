using System;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
  public class MultiMockingStrategy : AbstractMockingStrategy
  {
    private readonly Type[] _extraInterfaces;

    public MultiMockingStrategy(IAutoMockingRepository autoMock, Type[] extraInterfaces) : base(autoMock)
    {
      _extraInterfaces = extraInterfaces;
    }

    public override object Create(CreationContext context, Type type)
    {
      return Mocks.DynamicMultiMock(type, _extraInterfaces);
    }
  }
}