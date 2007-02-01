using System;

namespace Rhino.Generators.Definitions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PerfCounterCategoryAttribute : Attribute
	{
		private string category;

		public string Category
		{
			get { return category; }
		}

		public PerfCounterCategoryAttribute(string category)
		{
			this.category = category;
		}
	}
}