using System;
using System.Collections;
using Boo.Lang;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Rhino.Commons.Binsor
{
	public class Component : IQuackFu
	{
		private readonly IDictionary _parameters = new Hashtable();
		string _name;
		private readonly Type _service;
		private readonly Type _impl;
		
		public Component(string name, Type service)
			:this(name,service,service)
		{
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
		
		public Component(string name, Type service, Type impl, IDictionary parameters)
		 :this(name, service,impl)
		{
			_parameters = parameters;
		}

		public void Register()
		{
			IoC.Container.Kernel.AddComponent(_name,_service,_impl);
			if (_parameters.Count == 0)
				return;
			IHandler handler = IoC.Container.Kernel.GetHandler(_name);
			IoC.Container.Kernel.RegisterLiveDependencies(handler, _parameters);
		}

		public object QuackGet(string name)
		{
			return _parameters[name];
		}

		public object QuackSet(string name, object value)
		{
			return _parameters[name] = value;
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new NotSupportedException("You can't invoke a method on a component");
		}
	}
}