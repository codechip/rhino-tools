using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Rhino.Commons.LocalDataImpl
{
    public class LocalDataSwitcher : ILocalData
    {
        Action<ILocalData> set;

        public LocalDataSwitcher(Action<ILocalData> set)
        {
            this.set = set;
        }

        #region ILocalData Members

        public object this[object key]
        {
            get
            {
                ILocalData real = CreateAndSetLocalDataForEnvironment();
                return real[key];
            }
            set
            {
                ILocalData real = CreateAndSetLocalDataForEnvironment();
                real[key] = value;
            }
        }

        public void Clear()
        {
            ILocalData real = CreateAndSetLocalDataForEnvironment();
            real.Clear();
        }

        private ILocalData CreateAndSetLocalDataForEnvironment()
        {
            ILocalData real;
            if (HttpContext.Current != null)
                real = new CurrentRequestData();
            else
                real = new LocalThreadData();
            set(real);
            return real;
        }

        #endregion
    }
}
