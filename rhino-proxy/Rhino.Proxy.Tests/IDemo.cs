using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Proxy.Tests
{
    public interface IDemo
    {
        void Foo();
        void WithArg(string s);
        string Ret();
        void WithOutParam(out int i);
        void WithRefParam(ref int i);
    }
}
