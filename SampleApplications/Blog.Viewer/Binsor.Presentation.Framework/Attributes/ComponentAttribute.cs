namespace Binsor.Presentation.Framework.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ComponentAttribute : Attribute
	{
		private readonly string name;

		public ComponentAttribute(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}
	}
}