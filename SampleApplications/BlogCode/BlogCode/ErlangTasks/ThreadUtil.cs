namespace pipelines
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class ThreadUtil
    {
        private static readonly int processorCount = Environment.ProcessorCount;

        [DllImport("kernel32.dll")]
        private extern static void SwitchToThread();

        public static void Yield()
        {
            if(processorCount==1)
                SwitchToThread();
            else
                Thread.Sleep(1);
        }
    }
}