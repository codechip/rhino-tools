namespace Rhino.Commons.Binsor
{
	using System;
	using Boo.Lang;
	using Castle.Core.Configuration;
	using Configuration;

	public class Extend : IQuackFu, IConfigurationFormatter
	{
		private readonly Component component;

		public Extend(string name, params IComponentExtension[] extensions)
		{
			component = BooReader.GetComponentByName(name);
			component.AddExtensions(extensions);
		}

		public void Format(IConfiguration parent, string name, bool useAttribute)
		{
			component.Format(parent, name, useAttribute);
		}

		public object QuackGet(string name, object[] parameters)
		{
			return component.QuackGet(name, parameters);
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			return component.QuackSet(name, parameters, value);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return component.QuackInvoke(name, args);
		}
	}
}