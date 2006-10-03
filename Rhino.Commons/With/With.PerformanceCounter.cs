using System;
using System.Runtime.InteropServices;

namespace Rhino.Commons
{
  public static partial class With
  {
    public static double PerformanceCounter(Proc performance)
    {
      long start = 0, end = 0;

      QueryPerformanceCounter(out start);

      performance();

      QueryPerformanceCounter(out end);

      return (double)(end - start) / _frequency;
    }

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lpFrequency);

    private static readonly double _frequency = GetFrequency();

    private static double GetFrequency()
    {
      long frequency = 0;
      QueryPerformanceFrequency(out frequency);
      return (double)frequency;
    }
  }
}
