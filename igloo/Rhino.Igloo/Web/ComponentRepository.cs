#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
            List<BaseController> controllers = new List<BaseController>();
            foreach (PropertyInfo info in controllersToInject)
            {
                if(!info.CanWrite) return;

                BaseController controller = (BaseController)IoC.Resolve(info.PropertyType);
                controllers.Add(controller);
                info.SetValue(instance, controller, null);
            }

            foreach (BaseController controller in controllers)
            {
                controller.Initialize();
            }
        }
    }
}