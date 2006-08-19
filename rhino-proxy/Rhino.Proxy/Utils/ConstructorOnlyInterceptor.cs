using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Rhino.Proxy
{
    /// <summary>
    /// This is used by the proxies to handle 
    /// virtual method calls in base class constructors
    /// </summary>
    public static class ConstructorOnlyInterceptor
    {
        [ThreadStatic]
        static IInterceptor current;
        
        static string key = Guid.NewGuid().ToString();
        public static IInterceptor Current
        {
            get
            {
                if (HttpContext.Current == null)
                    return current ;
                else
                    return (IInterceptor)HttpContext.Current.Items[key]; 
            }
            set
            {
                if (HttpContext.Current == null)
                    current = value;
                else
                    HttpContext.Current.Items[key] = value;
            }
        }
    }
}
