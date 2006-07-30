using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;

namespace Rhino.Commons.LocalDataImpl
{
    public class CurrentRequestData : ILocalData
    {
        const string DataKey = "Rhino.Commons.LocalDataImpl.CurrentRequestData";
        #region ILocalData Members

        public object this[object key]
        {
            get
            {
                if(Current==null)
                    return null;
                return Current[key];
            }
            set
            {
             
                if (Current == null)
                   Current = new Hashtable();
               Current[key] = value;
            }
        }

        private static Hashtable Current
        {
            get { return HttpContext.Current.Items[DataKey] as Hashtable; }
            set { HttpContext.Current.Items[DataKey] = value; }
        }

        public void Clear()
        {
            Current.Clear();
        }

        #endregion
    }
}
