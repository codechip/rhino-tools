using System;
using System.Diagnostics;

namespace Rhino.Generators.Definitions
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PerformanceCounterAttribute : Attribute
	{
		private string name;
		private PerformanceCounterType counterType;

		public PerformanceCounterAttribute(PerformanceCounterType counterType)
		{
			this.counterType = counterType;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public PerformanceCounterType CounterType
		{
			get { return counterType; }
			set { counterType = value; }
		}
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PerformanceCounterCategoryAttribute : Attribute
	{
		private string category;


		public PerformanceCounterCategoryAttribute(string category)
		{
			this.category = category;
		}

		public string Category
		{
			get { return category; }
		}

	}
}
