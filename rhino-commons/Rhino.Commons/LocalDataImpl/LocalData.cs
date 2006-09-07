using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Rhino.Commons.LocalDataImpl;

namespace Rhino.Commons
{
    public class Local
    {
        static ILocalData current = new LocalData();
        static object LocalDataHashtableKey = new object();
        private class LocalData : ILocalData
        {
            [ThreadStatic]
            static Hashtable thread_hashtable;

            private static Hashtable Local_Hashtable
            {
                get
                {
                    if(HttpContext.Current==null)
                    {
                        return thread_hashtable ??
                        (
                            thread_hashtable = new Hashtable()
                        );
                    }
                    Hashtable web_hashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                    if(web_hashtable==null)
                    {
                        HttpContext.Current.Items[LocalDataHashtableKey] = web_hashtable = new Hashtable();
                    }
                    return web_hashtable;
                }
            }

            public object this[object key]
            {
                get { return Local_Hashtable[key]; }
                set { Local_Hashtable[key] = value; }
            }

            public void Clear()
            {
                Local_Hashtable.Clear();
            }
        }

        public static ILocalData Data
        {
            get { return current; }
        }
    }
}
