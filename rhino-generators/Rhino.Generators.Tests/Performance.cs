using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rhino.Generators.Definitions;

namespace Rhino.Generators.Tests
{
	[PerfCounterCategory("Extranet")]
	public abstract class Performance
	{
		[PerfCounter(PerformanceCounterType.NumberOfItems64)]
		public abstract PerformanceCounter TotalItems { get; }
	}
}
