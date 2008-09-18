using Rhino.Mocks;
using Rhino.Testing.AutoMocking;
using System;

namespace Rhino.Testing.Utility
{
    public abstract class BaseTest
    {
        private MockRepository mockery;
        private AutoMockingContainer container;

        protected AutoMockingContainer Container
        {
            get { return container; }
        }

        protected void InitializeContainer()
        {
            mockery = new MockRepository();
            container = new AutoMockingContainer(mockery);
            container.Initialize();
        }

        protected void DestroyContainer()
        {
            container.Dispose();
        }

        protected IDisposable Record
        {
            get { return mockery.Record(); }
        }

        protected IDisposable Playback
        {
            get { return mockery.Playback(); }
        }

        protected IDisposable PlaybackOnly
        {
            get 
            {
                using (Record) { }
                return mockery.Playback(); 
            }
        }

        protected T Mock<T>() where T : class
        {
            return container.Get<T>();
        }

        protected Item CreateSUT<Item>()
        {
            return container.Create<Item>();
        }

        protected void ProvideAnImplementationOf<Interface>(object instance)
        {
            container.AddService(typeof(Interface), instance);
        }
    }
}
