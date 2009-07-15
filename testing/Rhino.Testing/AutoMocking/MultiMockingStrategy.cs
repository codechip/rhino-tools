using System;
using System.Collections.Generic;
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
      var types = new List<Type>(_extraInterfaces.Length + 1);
      types.Add(type);
      types.AddRange(_extraInterfaces);

      return MockFactory.GenerateDynamicMock(types.ToArray());
    }
  }
}