using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public class EventArgs<T> : EventArgs
    {
        T item;

        public EventArgs(T item)
        {
            this.item = item;
        }

        public T Item
        {
            get { return item; }
            set { item = value; }
        }


    }
}
