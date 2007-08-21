using System;
using Castle.MicroKernel;

namespace Rhino.Testing.Tests.AutoMocking
{
    public class DefaultCollectionOfServices : ICollectionOfServices
    {
        #region ICollectionOfServices Members

        public IDisposable SomethingToDispose
        {
            get { return null; }
			set { }
        }

        #endregion
    }
}