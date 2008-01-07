namespace pipelines
{
    using System;

    public class SchedulingOptions
    {
        [ThreadStatic]
        public static int NestedStackCount;
    }
}