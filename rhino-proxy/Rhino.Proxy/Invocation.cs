using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rhino.Proxy
{
    public class Invocation : IInvocation
    {
        private readonly MethodInvocationDelegate invocationDelegate;
        private MethodInfo method;
        private object invocationTarget;
        private object proxy;
    	private object originalTarget;
#if DOTNET2
    	private Type[] typeParameters;
#endif
    	public Invocation(MethodInvocationDelegate invocationDelegate,
    	                  MethodInfo method,  
#if DOTNET2
    	                  Type[] typeParameters,
#endif
    	                  object proxy, object originalTarget)
        {
            this.invocationDelegate = invocationDelegate;
            this.method = method;
#if DOTNET2
    		this.typeParameters = typeParameters;
#endif
            this.proxy = proxy;
    		this.originalTarget = originalTarget;
        }

        public object Proxy
        {
            get { return proxy; }
        }

        public object InvocationTarget
        {
            get { return invocationTarget ?? originalTarget; }
            set { invocationTarget = value; }
        }

        public MethodInfo Method
        {
            get { return method; }
        }
#if DOTNET2
    	public Type[] TypeParameters
    	{
			get { return typeParameters; }
    	}
#endif
    	public object Proceed(params object[] args)
        {
            // If the user changed the target, we use reflection
            // otherwise the delegate will be used.
            if(invocationTarget == null)
            {
                return invocationDelegate.Invoke(args);
            }
            else
            {
                return Method.Invoke(InvocationTarget, args);
            }
        }

        public MethodInfo MethodInvocationTarget
        {
            get { return method; }
        }
    }
}
