using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Castle.MicroKernel;
using log4net;
using Rhino.Commons;
using Rhino.Commons.Web;

namespace Extranet.Infrastructure.Web
{
    /// <summary>
    /// A UI component that keeps all infos to do injection. 
    /// </summary>
    public sealed class UIComponent
    {
        private ILog logger = LogManager.GetLogger(typeof(UIComponent));

        /// <summary>
        /// Binding token
        /// </summary>
        private BindingFlags BINDING_FLAGS_SET
            = BindingFlags.Public
              | BindingFlags.SetProperty
              | BindingFlags.Instance
              | BindingFlags.SetField
                             ;

        public const string ComponentSuffix = ".uicomponent";
        public const string ViewSuffix = "view.";
        public const string UIComponentToRefresh = "_UIComponentToRefresh_";

        private string _name = string.Empty;
        private Type _componentType = null;
        private IDictionary<InjectAttribute, PropertyInfo> _inMembers = new Dictionary<InjectAttribute, PropertyInfo>();
        private IDictionary<OutjectAttribute, PropertyInfo> _outMembers = new Dictionary<OutjectAttribute, PropertyInfo>();
        private IList<PropertyInfo> _inControllers = new List<PropertyInfo>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }


        /// <summary>
        /// Gets the in members (properties or fields).
        /// </summary>
        /// <value>The in properties.</value>
        public IDictionary<InjectAttribute, PropertyInfo> InMembers
        {
            get { return _inMembers; }
        }

        /// <summary>
        /// Gets the out (properties or fields).
        /// </summary>
        /// <value>The out properties.</value>
        public IDictionary<OutjectAttribute, PropertyInfo> OutMembers
        {
            get { return _outMembers; }
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public Type ComponentType
        {
            get { return _componentType; }
        }

        /// <summary>
        /// Gets a value indicating whether [needs injection].
        /// </summary>
        /// <value><c>true</c> if [needs injection]; otherwise, <c>false</c>.</value>
        public bool NeedsInjection
        {
            get { return _inMembers.Count > 0; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIComponent"/> class.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        public UIComponent(Type componentType)
        {
            Validation.NotNull(componentType, "componentType");

            _name = componentType.FullName + ComponentSuffix; ;
            _componentType = componentType;

            InitMembers();
        }


        /// <summary>
        /// Inject context variable values into [Inject] attributes
        /// of a component instance.
        /// </summary>
        /// <param name="instance">A UI component instance.</param>
        public void Inject(Object instance)
        {
            InjectMembers(instance, true);
            InjectIOCComponnet(instance);
        }

        /// <summary>
        /// Injects the members.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="trackInjectedMembers">
        /// if set to <c>true</c> tracks injected members so they can be refresh them on back call from controller method.
        /// </param>
        /// <remarks>TO DO Inject fields</remarks>
        public void InjectMembers(Object instance, bool trackInjectedMembers)
        {
            if (NeedsInjection)
            {
                logger.DebugFormat("Injecting dependencies of: {0} ", _name);


                if (trackInjectedMembers)
                {
                    // Tracks the injected members so bijection interceptor
                    // can refresh them on back call from controller method.
                    //UIComponent, object
                    IDictionary<UIComponent, object> uiComponentToRefresh = (IDictionary<UIComponent, object>)Scope.Flash[UIComponent.UIComponentToRefresh];
                    if (uiComponentToRefresh == null)
                    {
                        uiComponentToRefresh = new Dictionary<UIComponent, object>();
                        Scope.Flash[UIComponentToRefresh] = uiComponentToRefresh;
                    }
                    uiComponentToRefresh.Add(this, instance);
                }

                foreach (KeyValuePair<InjectAttribute, PropertyInfo> kvp in InMembers)
                {
                    PropertyInfo propertyInfo = kvp.Value;
                    object instanceToInject = Scope.Any[kvp.Key.Name];

                    if (instanceToInject == null)
                    {
                        if (kvp.Key.Required)
                            throw new InjectionException(
                                string.Format(CultureInfo.InvariantCulture,"Could not find value in scopes for required member '{0}' named '{1}' on '{2}'",
                                              kvp.Value.Name, kvp.Key.Name, kvp.Value.DeclaringType.FullName));
                    }
                    propertyInfo.SetValue(instance, instanceToInject, null);
                }
            }
        }

        private void InjectIOCComponnet(Object instance)
        {
            // Inject controllers
            foreach (PropertyInfo propertyInfo in _inControllers)
            {
                object controller = IoC.Container[propertyInfo.PropertyType];
                propertyInfo.SetValue(instance, controller, null);
            }
        }


        private void InitMembers()
        {
            _name = ViewSuffix + _name;
            RetrieveInjectedProperties();
        }

        /// <summary>
        /// Retrieves the injected user context scoped object and
        /// injected IController components on a view component
        /// </summary>
        /// <remarks>
        /// Today, only check injected properties memnbers
        /// TO DO also check injected fields 
        /// </remarks>
        private void RetrieveInjectedProperties()
        {
            PropertyInfo[] properties = _componentType.GetProperties(BINDING_FLAGS_SET);

            for (int i = 0; i < properties.Length; i++)
            {
                if (!typeof(BaseController).IsAssignableFrom(properties[i].PropertyType))
                {
                    InjectAttribute injectAttribute = AttributeUtil.GetInjectAttribute(properties[i]);
                    if (injectAttribute != null)
                    {
                        if (injectAttribute.Name.Length == 0)
                        {
                            injectAttribute.Name = properties[i].Name;
                        }
                        _inMembers.Add(injectAttribute, properties[i]);
                    }
                }
            }

            // Retrieves IOC setter injection
            for (int i = 0; i < properties.Length; i++)
            {
                if ((typeof(BaseController).IsAssignableFrom(properties[i].PropertyType)))
                {
                    _inControllers.Add(properties[i]);
                }
            }
        }
    }
}