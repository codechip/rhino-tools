using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using Castle.Core;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
	public class Component : IQuackFu
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
			BooReader.Components.Add(this);
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

		public void Register()
		{
			IKernel kernel = IoC.Container.Kernel;
			kernel.AddComponent(_name, _service, _impl);
			IHandler handler = kernel.GetHandler(_name);
			handler.ComponentModel.LifestyleType = lifestyle;
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

	public class ComponentReference
	{
		string name;

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