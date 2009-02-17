using System;

namespace Rhino.Testing.Tests.AutoMocking
{
    public interface ICollectionOfServices
    {
		IDisposable SomethingToDispose { get; set;  }
    }
}