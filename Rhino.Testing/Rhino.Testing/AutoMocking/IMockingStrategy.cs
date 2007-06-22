using System;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    public interface IMockingStrategy
    {
        object Create(CreationContext context, Type type);
    }
}