using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Commons.LocalDataImpl;

namespace Rhino.Commons
{
    public class Local
    {
        static ILocalData current = new LocalDataSwitcher(delegate(ILocalData real) { current = real; } );

        public static ILocalData Data
        {
            get { return current; }
        }
    }
}
