namespace Rhino.Commons.Binsor
{
	using System;
	using Castle.Core.Configuration;
	using Configuration;

	public class ComponentReference : IConfigurationFormatter, INeedSecondPassRegistration
	{
		private readonly string _name;
		private readonly Type _service;

		public ComponentReference(string name)
		{
			_name = name;
		}

		public ComponentReference(Type service)
		{
			_name = service.FullName;
			_service = service;
			BooReader.NeedSecondPassRegistrations.Add(this);
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

		public void RegisterSecondPass()
		{
			if (!AbstractConfigurationRunner.IoC.Container.Kernel.HasComponent(_name))
			{
				AbstractConfigurationRunner.IoC.Container.AddComponent(_name, _service);
			}
		}
	}
}