namespace Rhino.Commons.Test
{
    using System;
    using MbUnit.Framework;

    public class BaseTest
    {
        public virtual void TestInitialize()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(Environment.CurrentDirectory);
        }
    }
}