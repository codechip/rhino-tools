using System;
using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel;
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
    private bool _resolveProperties;
    private readonly IMockFactory _mockFactory;

    /// <summary>Creates an AutoMocking Container that defaults to using the AAA mocking syntax.
    /// Many features are unavailable in the mode so use with cauting.</summary>
    public AutoMockingContainer() : this(null, false)
    {
    }

		public AutoMockingContainer(MockRepository mocks) : this(mocks, false)
		{
		}

    //public AutoMockingContainer(MockRepository mocks, bool resolveProperties)
    //  : this(new MockRepositoryAdapter(mocks), resolveProperties)
    //{
    //}

    /// <summary>Creates an AutoMocking Container with the given <paramref name="mocks"/> repository.</summary>
    /// <param name="mocks">The mocks repository to use</param>
    /// <param name="resolveProperties"><c>true</c> if properties should be resolved, otherwise <c>false</c>; the defualt is <c>false</c></param>
    public AutoMockingContainer(MockRepository mocks, bool resolveProperties)
    {
      if (mocks == null)
      {
        _mockFactory = new AAAMockFactory();
      }
      else
      {
        _mockFactory = new DefaultMockFactory(mocks);
      }
      _mocks = mocks;
      _resolveProperties = resolveProperties;
    }
        #region IAutoMockingRepository Members

    public IMockFactory MockFactory
    {
      get { return _mockFactory; }
    }

    public virtual MockRepository MockRepository
		{
			get { return _mocks ?? new MockRepository(); }
		}

		private new object GetService(Type type) 
		{
			if (_services.ContainsKey(type))
				return _services[type];
			return null;
		}

		#region DefaultMockingStrategy
		private IMockingStrategy m_DefaultMockingStrategy;

    /// <summary>
		/// Gets or sets the default mocking strategy., which will be returned, if a <see cref="Type"/> was not explicitly marked via a <see cref="TypeMarker"/>.
		/// The default is the <see cref="DynamicMockingStrategy"/>,
		/// which will always be returned, if no other was defined.
		/// </summary>
		/// <value>The default mocking strategy.</value>
		public IMockingStrategy DefaultMockingStrategy
		{
			get
			{
				if (m_DefaultMockingStrategy == null)
				{
					m_DefaultMockingStrategy = new DynamicMockingStrategy(this);
				}
				return m_DefaultMockingStrategy;
			}
			set
			{
				m_DefaultMockingStrategy = value;
			}
		}
		#endregion

		public virtual IMockingStrategy GetMockingStrategy(Type type)
		{
			if (_strategies.ContainsKey(type))
			{
				return _strategies[type];
			}
			return DefaultMockingStrategy;
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

		/// <summary>
		/// Initializes this instance. Must be called, before you can work with the instance.
		/// </summary>
		public void Initialize()
		{
			Kernel.AddSubSystem(SubSystemConstants.NamingKey,new AutoMockingNamingSubSystem(this));
			Kernel.AddFacility("AutoMockingFacility", new AutoMockingFacility(this));
		    Kernel.ComponentModelBuilder.AddContributor(new NonPublicConstructorDependenciesModelInspector());
			Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
		}

		void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
		{
			if (model.CustomComponentActivator!=null)
				return;
		    model.CustomComponentActivator = ResolveProperties
		                                         ? typeof (AutoMockingComponentActivator)
		                                         : typeof (NonPropertyResolvingComponentActivator);
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
        /// Indicate that instances of <paramref name="type"/> should not be resolved by the container
        /// <seealso cref="Create{T}()"/>
        /// </summary>
		public void MarkMissing(Type type)
		{
			_markMissing.Add(type);
		}

	    public bool ResolveProperties
	    {
            get { return _resolveProperties; }
            set { _resolveProperties = value; }
	    }
	}
}
