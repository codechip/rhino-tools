using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Rhino.Generators.Definitions;

namespace Rhino.Generators.Tests
{
	[PerformanceCounterCategory("TestFoo")]
	public abstract class Performance
	{
		[PerformanceCounter(PerformanceCounterType.NumberOfItems64)]
		public abstract PerformanceCounter TotalItems { get; }
	}
}
