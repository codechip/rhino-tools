using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Rhino.Commons.Logging
{
    public class WebLoggingHelper
    {
        public static object CurrentPage = new CurrentPageHelper();
        public static object CurrentRequestId = new CurrentRequestIdHelper();
        
        class CurrentRequestIdHelper
        {
            public override string ToString()
            {
                if (HttpContext.Current == null)
                {
                    return "no request";
                }
                return HttpContext.Current.GetHashCode().ToString();
            }
        }
        
        class CurrentPageHelper
        {
            public override string ToString()
            {
                if (HttpContext.Current == null)
                {
                    return "not in a page";
                }
                return HttpContext.Current.Request.RawUrl;
            }

        }
    }
}
