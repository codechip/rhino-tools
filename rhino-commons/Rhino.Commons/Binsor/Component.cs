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
using Boo.Lang;
using Castle.Core;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
	public class Component : IQuackFu, INeedSecondPassRegistration
	{
		private readonly IDictionary _attributes = new Hashtable();
		private readonly IDictionary _parameters = new Hashtable();
		private readonly Dictionary<string, string> _references = new Dictionary<string, string>();
		private readonly string _name;
		private readonly LifestyleType lifestyle = LifestyleType.Singleton;
		private readonly Type _service;
		private readonly Type _impl;

		public Component(string name, Type service) : this(name, service, service)
		{
		}

		public Component(string name, Type service, Type impl, LifestyleType lifestyleType) : this(name, service, impl)
		{
			this.lifestyle = lifestyleType;
		}

		public Component(string name, Type service, Type impl)
		{
			this._name = name;
			_service = service;
			_impl = impl;
		    BooReader.NeedSecondPassRegistrations.Add(this);
		}

		public Component(string name, Type service, IDictionary parameters) : this(name, service)
		{
			_parameters = parameters;
		}

		public Component(string name, Type service, Type impl, IDictionary parameters, LifestyleType lifestyleType)
			: this(name, service, impl, parameters)
		{
			this.lifestyle = lifestyleType;
		}

		public Component(string name, Type service, Type impl, IDictionary parameters) : this(name, service, impl)
		{
			_parameters = parameters;
		}

		public string Name
		{
			get { return _name; }
		}

        public Component Register()
		{
			IKernel kernel = IoC.Container.Kernel;
			kernel.AddComponent(_name, _service, _impl);
			IHandler handler = kernel.GetHandler(_name);
			handler.ComponentModel.LifestyleType = lifestyle;
            return this;
		}

        public void RegisterSecondPass()
        {
            IKernel kernel = IoC.Container.Kernel;
            IHandler handler = kernel.GetHandler(_name);
            kernel.RegisterCustomDependencies(_name, _parameters);
            foreach (KeyValuePair<string, string> pair in _references)
            {
                handler.ComponentModel.Parameters.Add(pair.Key, pair.Value);
            }
        }

		public object QuackGet(string name, object[] property_parameters)
		{
			if(property_parameters!=null && property_parameters.Length>0)
			{
				return _attributes[property_parameters[0]];
			}
			return _parameters[name];
		}

		public object QuackSet(string name, object[] property_parameters, object value)
		{
			if (value is ComponentReference)
			{
				string referenceName = ((ComponentReference) value).Name;
				_references.Add(name, referenceName);
				return null;
			}
			if(property_parameters!=null && property_parameters.Length>0)
			{
				_attributes[property_parameters[0]] = value;
				return null;
			}
			return _parameters[name] = value;
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new NotSupportedException("You can't invoke a method on a component");
		}
	}

    public interface INeedSecondPassRegistration
    {
        void RegisterSecondPass();
    }

    public class ComponentReference
	{
	    readonly string name;

		public string Name
		{
			get { return this.name; }
		}

		public ComponentReference(string name)
		{
			this.name = "${" + name + "}";
		}
	}
}