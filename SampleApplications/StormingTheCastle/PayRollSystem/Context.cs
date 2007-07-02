using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Commons;

namespace PayRollSystem
{
    public static class Context
    {
        public static IDisposable Enter(string name)
        {
            return IoC.Resolve<ContainerSelector>().Enter(name);
        }
    }
}
