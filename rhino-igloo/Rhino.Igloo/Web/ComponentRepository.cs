using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Commons;
using Rhino.Igloo;

namespace Rhino.Igloo
{
	/// <summary>
	/// Holds all the ComponentCache for the UI components in the application.
	/// </summary>
    public sealed class ComponentRepository
    {
        private IDictionary<Type, IList<PropertyInfo>> _componentsByType = new Dictionary<Type, IList<PropertyInfo>>();


        /// <summary>
        /// Adds the ComponentCache.
        /// </summary>
        /// <param myName="ComponentCache">The ComponentCache.</param>
        public void AddComponent(Type type)
        {
            IList<PropertyInfo> list = RetrieveInjectedProperties(type);
            _componentsByType.Add(type, list);
        }

        /// <summary>
        /// Find properties that require injection of BaseController components on a view component
        /// </summary>
        private static IList<PropertyInfo> RetrieveInjectedProperties(Type viewType)
        {
            PropertyInfo[] properties = viewType.GetProperties(BindingFlags.Public|BindingFlags.Instance);
            List<PropertyInfo> controllersToInject = new List<PropertyInfo>();
            // Retrieves IOC controller setter injection
            for (int i = 0; i < properties.Length; i++)
            {
                if ((typeof(BaseController).IsAssignableFrom(properties[i].PropertyType)))
                {
                    controllersToInject.Add(properties[i]);
                }
            }
            return controllersToInject;
        }


        /// <summary>
        ///  Inject controllers to the view instance
        /// </summary>
        public void InjectControllers(object instance)
        {
            Type type = instance.GetType();
            IList<PropertyInfo> controllersToInject;
            if (!(_componentsByType.TryGetValue(type, out controllersToInject) ||
                _componentsByType.TryGetValue(type.BaseType, out controllersToInject)))
                return;

            foreach (PropertyInfo info in controllersToInject)
            {
                object controller = IoC.Container.Resolve(info.PropertyType);
                info.SetValue(instance, controller, null);
            }
        }
    }
}