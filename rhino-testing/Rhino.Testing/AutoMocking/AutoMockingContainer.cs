using System;
using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;
using Castle.Windsor;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
	public class AutoMockingContainer : WindsorContainer, IAutoMockingRepository, IGenericMockingRepository
	{
		private readonly IList<Type> _markMissing = new List<Type>();
		private readonly MockRepository _mocks;
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
		private readonly Dictionary<Type, IMockingStrategy> _strategies = new Dictionary<Type, IMockingStrategy>();

		public AutoMockingContainer(MockRepository mocks)
		{
			_mocks = mocks;
		}

		#region IAutoMockingRepository Members
		public virtual MockRepository MockRepository
		{
			get { return _mocks; }
		}

		private new object GetService(Type type) 
		{
			if (_services.ContainsKey(type))
				return _services[type];
			return null;
		}

		public virtual IMockingStrategy GetMockingStrategy(Type type)
		{
			if (_strategies.ContainsKey(type))
			{
				return _strategies[type];
			}
			return new DynamicMockingStrategy(this);
		}

		public virtual IMockingStrategy GetMockingStrategy<T>()
		{
			return GetMockingStrategy(typeof (T));
		}

		public bool CanResolve(Type type)
		{
			return _markMissing.Contains(type) == false;
		}
		#endregion

		public void Initialize()
		{
			Kernel.AddSubSystem(SubSystemConstants.NamingKey,new AutoMockingNamingSubSystem(this));
			Kernel.AddFacility("AutoMockingFacility", new AutoMockingFacility(this));
			Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
		{
			if (model.CustomComponentActivator == null)
			{
				model.CustomComponentActivator = typeof(AutoMockingComponentActivator);
			}
		}

		private void AddComponentIfMissing<T>()
		{
			Type targetType = typeof(T);
			if (!Kernel.HasComponent(targetType.FullName))
				AddComponent(targetType.FullName, targetType);
		}

        /// <summary>
        /// Create an instance of type <typeparamref name="T"/> with its constructor declared
        /// dependencies resolved as mocks
        /// </summary>
        /// <remarks>
        /// For each constructor dependency that <typeparamref name="T"/> declares, use <see
        /// cref="Mark"/> to determine the type of mock (stub, dynamic mock, etc) that will be
        /// injected into the instance. The default is to inject a dynmaic mock.
        /// <para>
        /// Use <see cref="MarkMissing"/> for a constructor dependency that you do not want the
        /// container to resolve. 
        /// </para>
        /// <para>
        /// If you want a dependency to be resolved as an explicit type rather than as a mock, use
        /// <see cref="IWindsorContainer.AddComponent(string,Type,Type)"/> to register the explict
        /// type that the container should use before calling this method.
        /// </para>
        /// </remarks>
		public T Create<T>()
		{
			AddComponentIfMissing<T>();
			return Resolve<T>();
		}

        /// <summary>
        /// See <see cref="Create{T}()"/>
        /// </summary>
		public T Create<T>(IDictionary parameters)
		{
			AddComponentIfMissing<T>();
			return Resolve<T>(parameters);
		}


        /// <summary>
        /// Returns a mock of the specified <paramref name="type"/>
        /// </summary>
        /// <remarks>
        /// Use <see cref="Mark"/> to determine the type of mock (stub, dynamic mock, etc) that will be
        /// returned. A dynamic mock will be returned by default
        /// <para>
        /// Only a single instance of the specified <paramref name="type"/> will be created and returned
        /// </para>
        /// </remarks>
		public object Get(Type type)
		{
			if (type == typeof(IKernel))
				return Kernel;
			object t = GetService(type);
			if (t != null)
				return t;

			object instance = GetMockingStrategy(type).Create(CreationContext.Empty, type);
			AddService(type, instance);
			return instance;
		}

        /// <summary>
        /// See <see cref="Get"/>
        /// </summary>
		public T Get<T>() where T : class
		{
			return (T) Get(typeof (T));
		}

		public void SetMockingStrategy(Type type, IMockingStrategy strategy)
		{
			_strategies[type] = strategy;
		}

		public void SetMockingStrategy<T>(IMockingStrategy strategy)
		{
			SetMockingStrategy(typeof(T), strategy);
		}

		public void AddService(Type type, object service)
		{
			_services[type] = service;
		}
		
		public void AddService<T>(T service)
		{
			AddService(typeof (T), service);
		}

        /// <summary>
        /// See <see cref="IAutoMockingRepository.Mark"/>
        /// </summary>
		public TypeMarker Mark(Type type)
		{
			return new TypeMarker(type, this);
		}

		public bool CanResolveFromMockRepository(Type service)
		{
			return _markMissing.Contains(service) == false && 
				GetMockingStrategy(service).GetType() != typeof (NonMockedStrategy);
		}

        /// <summary>
        /// See <see cref="IAutoMockingRepository.Mark"/>
        /// </summary>
		public TypeMarker Mark<T>()
		{
			return Mark(typeof (T));
		}

        /// <summary>
        /// See <see cref="MarkMissing"/>
        /// </summary>
		public void MarkMissing<T>()
		{
			MarkMissing(typeof (T));
		}

        /// <summary>
        /// Indictate that instances of <paramref name="type"/> should not be resolved by the container
        /// <seealso cref="Create{T}()"/>
        /// </summary>
		public void MarkMissing(Type type)
		{
			_markMissing.Add(type);
		}
	}
}
