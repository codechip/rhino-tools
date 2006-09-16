using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons.Test.Components
{
    public class Fubar
    {
        object foo;

        public object Foo
        {
            get { return foo; }
        }

        public Fubar(object foo)
        {
            this.foo = foo;
        }
    }
}
