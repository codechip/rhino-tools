using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons.LocalDataImpl
{
    public class LocalThreadData : ILocalData
    {
        [ThreadStatic]
      private static Hashtable data;

        public static Hashtable Data
      {
        get
        { 
          if(data==null)
              data = new Hashtable();
          return LocalThreadData.data; 
        }
      }

        #region ILocalData Members

        public object this[object key]
        {
            get
            {
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }

        public void Clear()
        {
            Data.Clear();
        }

        #endregion
    }
}
