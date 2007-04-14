using System;
using System.Collections.Generic;

namespace Extranet.Infrastructure.Web
{
    public sealed class UIComponentRepository
    {
        private IDictionary<Type, UIComponent> _componentsByType = new Dictionary<Type, UIComponent>();
        private IDictionary<string, UIComponent> _componentsByName = new Dictionary<string, UIComponent>();


        /// <summary>
        /// Adds the uiComponent.
        /// </summary>
        /// <param name="uiComponent">The uiComponent.</param>
        public void AddComponent(UIComponent uiComponent)
        {
            if (uiComponent == null)
            {
                throw new ArgumentNullException("uiComponent");
            }
            _componentsByType.Add(uiComponent.ComponentType, uiComponent);
            _componentsByName.Add(uiComponent.Name, uiComponent);
        }



        /// <summary>
        /// Gets the component for this name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The found component</returns>
        public UIComponent ComponentForName(string name)
        {
            return _componentsByName[name + UIComponent.ComponentSuffix];
        }


        /// <summary>
        ///  Gets the component for this type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The found component</returns>
        public UIComponent ComponentForType(Type type)
        {
            return _componentsByType[type];
        }
    }
}