using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Proxy.Tests
{
    public class TestInterceptor : IInterceptor 
    {
        public ArrayList Invocations = new ArrayList();
        public object returnValue = null;
        public object [] setArgs = new object[0];
        public object Intercept(IInvocation invocation, params object[] args)
        {
            Invocations.Add(new InvocationAndArgs(invocation, args));
            int i = 0;
            foreach (object setArg in setArgs)
            {
                args[i] = setArg;
                i += 1;
            }
            return returnValue;
        }
    }
    
    public class InvocationAndArgs
    {
        IInvocation invocation;
        object[] args;

        public IInvocation Invocation
        {
            get { return invocation; }
        }

        public object[] Args
        {
            get { return args; }
        }

        public InvocationAndArgs(IInvocation invocation, object[] args)
        {
            this.invocation = invocation;
            this.args = args;
        }
    }
}
