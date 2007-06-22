using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.Windsor;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public class AutoMockingContainer : WindsorContainer, IAutoMockingRepository
    {
        private IList<Type> _markMissing = new List<Type>();
        private MockRepository _mocks;
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private Dictionary<Type, IMockingStrategy> _strategies = new Dictionary<Type, IMockingStrategy>();

        #region IAutoMockingRepository Members

        public virtual MockRepository MockRepository
        {
            get { return _mocks; }
        }

        public virtual void AddService(Type type, object service)
        {
            _services[type] = service;
        }

        public virtual object Get(Type type)
        {
            if (_services.ContainsKey(type))
                return _services[type];
            return null;
        }

        public virtual T Get<T>()
        {
            return (T) Get(typeof (T));
        }

        public virtual IMockingStrategy GetMockingStrategy(Type type)
        {
            if (_strategies.ContainsKey(type))
            {
                return _strategies[type];
            }
            return new DynamicMockingStrategy(this);
        }

        public void MarkStub(Type type)
        {
            _strategies[type] = new StubbedStrategy(this);
        }

        public void MarkNonDynamic(Type type)
        {
            _strategies[type] = new StandardMockingStrategy(this);
        }

        public void MarkNonMocked(Type type)
        {
            _strategies[type] = new NonMockedStrategy(this);
        }

        public void MarkMissing(Type type)
        {
            _markMissing.Add(type);
        }


        public bool CanResolve(Type type)
        {
            return _markMissing.Contains(type) == false;
        }

        #endregion

        public void Initialize()
        {
            Kernel.AddFacility("AutoMockingFacility", new AutoMockingFacility(this));
        }

        public T Create<T>(MockRepository mocks)
        {
            Type targetType = typeof (T);
            _mocks = mocks;
            AddComponent(targetType.FullName, targetType);
            return Resolve<T>();
        }

        public T GetMock<T>()
            where T : class
        {
            T t = Get<T>();
            if (t != null)
                return t;
            object instance = _strategies[typeof (T)].Create(CreationContext.Empty, typeof (T));
            AddService(typeof (T), instance);
            return (T) instance;
        }

        public void MarkStub<T>()
        {
            MarkStub(typeof (T));
        }

        public void MarkNonDynamic<T>()
        {
            MarkNonDynamic(typeof (T));
        }

        public void MarkNonMocked<T>()
        {
            MarkNonMocked(typeof (T));
        }

        public void MarkMissing<T>()
        {
            MarkMissing(typeof (T));
        }
    }
}