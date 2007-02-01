using System;
using System.Diagnostics;

namespace Rhino.Generators.Definitions
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PerfCounterAttribute : Attribute
	{
		private readonly string name;
		private PerformanceCounterType counterType;


		public PerfCounterAttribute(string name, PerformanceCounterType counterType)
		{
			this.name = name;
			this.counterType = counterType;
		}


		public PerfCounterAttribute(PerformanceCounterType counterType)
		{
			this.counterType = counterType;
		}

		public string Name
		{
			get { return name; }
		}

		public PerformanceCounterType CounterType
		{
			get { return counterType; }
		}
	}
}
