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
using Boo.Lang;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
	public class Component : IQuackFu, INeedSecondPassRegistration, IConfigurationFormatter
	{
		private readonly string _name;
		private readonly Type _service;
		private readonly Type _impl;
		private readonly IDictionary _attributes = new Hashtable();
		private readonly IDictionary _dependencies = new Hashtable();
		private readonly LifestyleType _lifestyle = LifestyleType.Singleton;
		private IConfiguration _configuration;
		private IConfiguration _parameters;

        //important, we need to get this when we create the component, because we need
        //to support nested components
	    private readonly IKernel kernel = IoC.Container.Kernel;

	    public Component(string name, Type service) : this(name, service, service)
		{
		}

		public Component(string name, Type service, Type impl, LifestyleType lifestyleType) 
			: this(name, service, impl)
		{
			_lifestyle = lifestyleType;
		}

		public Component(string name, Type service, Type impl)
		{
			_name = name;
			_service = service;
			_impl = impl;
		    BooReader.NeedSecondPassRegistrations.Add(this);
		}

		public Component(string name, Type service, IDictionary configuration)
			: this(name, service)
		{
			CreateConfiguration(configuration);
		}

		public Component(string name, Type service, Type impl, IDictionary configuration, LifestyleType lifestyleType)
			: this(name, service, impl, configuration)
		{
			_lifestyle = lifestyleType;
		}

		public Component(string name, Type service, Type impl, IDictionary configuration) 
			: this(name, service, impl)
		{
			CreateConfiguration(configuration);
		}

		public string Name
		{
			get { return _name; }
		}

        public Component Register()
		{
			if (_configuration != null)
			{
				kernel.ConfigurationStore.AddComponentConfiguration(_name, _configuration);
			}
        	kernel.AddComponent(_name, _service, _impl, _lifestyle);
            return this;
		}

        public void RegisterSecondPass()
        {
            kernel.RegisterCustomDependencies(_name, _dependencies);
        }

		public object QuackGet(string name, object[] property_dependencies)
		{
			if(property_dependencies!=null && property_dependencies.Length>0)
			{
				return _attributes[property_dependencies[0]];
			}
			return _dependencies[name];
		}

		public object QuackSet(string name, object[] property_dependencies, object value)
		{
			if (ConfigurationHelper.RequiresConfiguration(value))
			{
				EnsureParametersConfiguration();
				ConfigurationHelper.ConvertDependencyToConfiguration(_parameters, name, value);
				return null;
			}

			if(property_dependencies!=null && property_dependencies.Length>0)
			{
				_attributes[property_dependencies[0]] = value;
				return null;
			}

			return _dependencies[name] = value;
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new NotSupportedException("You can't invoke a method on a component");
		}

		public void Format(IConfiguration parent, string name, bool useAttribute)
		{
			if (useAttribute)
			{
				parent.Attributes.Add(name, _name);
			}
			else
			{
				new ComponentReference(this).Format(parent, name, useAttribute);
			}
		}

		private void CreateConfiguration(IDictionary configuration)
		{
			_configuration = ConfigurationHelper.CreateConfiguration(null, "component", configuration);
		}

		private void EnsureParametersConfiguration()
		{
			if (_parameters == null)
			{
				if (_configuration == null)
				{
					CreateConfiguration(null);
				}
				else
				{
					foreach (IConfiguration child in _configuration.Children)
					{
						if (child.Name.Equals("parameters", StringComparison.InvariantCultureIgnoreCase))
						{
							_parameters = child;
							return;
						}
					}
				}

				_parameters = new MutableConfiguration("parameters");
				_configuration.Children.Add(_parameters);
			}
		}
	}

    public interface INeedSecondPassRegistration
    {
        void RegisterSecondPass();
    }

    public class ComponentReference : IConfigurationFormatter
	{
	    private readonly string _name;

		public ComponentReference(string name)
		{
			_name = name;
		}

		public ComponentReference(Component component)
			: this(component.Name)
		{
		}

		public string Name
		{
			get { return _name; }
		}

		public void Format(IConfiguration parent, string name, bool useAttribute)
		{
			if (useAttribute)
			{
				parent.Attributes.Add(name, _name);
			}
			else
			{
				string reference = "${" + _name + "}";
				ConfigurationHelper.CreateChild(parent, name, reference);
			}
		}
	}
}