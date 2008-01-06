namespace Rhino.Commons
{
    using System;

    public static class Guard
    {
        public static void Against(bool assertion, string message)
        {
            if(assertion == false)
                return;
            throw new InvalidOperationException(message);
        }

        public static void Against<T>(bool assertion, string message) where T : Exception
        {
            if (assertion == false)
                return;
            throw (T)Activator.CreateInstance(typeof (T), message);
        }
    }
}