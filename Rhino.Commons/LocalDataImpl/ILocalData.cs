using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons.LocalDataImpl
{
    public interface ILocalData
    {
        object this[object key] { get; set; }

        void Clear();
    }
}
