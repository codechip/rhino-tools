using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public class PropertyIndexer<RetType, IndexType>
    {
        private Func<RetType, IndexType> getter;
        private Proc<IndexType, RetType> setter;

        public PropertyIndexer(Func<RetType, IndexType> getter, Proc<IndexType, RetType> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public RetType this[IndexType index]
        {
            get { return getter(index); }
            set { setter(index, value); }
        }
    }
    public class PropertyIndexerGetter<RetType, IndexType>
    {
        private Func<RetType, IndexType> getter;

        public PropertyIndexerGetter(Func<RetType, IndexType> getter)
        {
            this.getter = getter;
        }

        public RetType this[IndexType index]
        {
            get { return getter(index); }
        }
    }

    public class PropertyIndexerSetter<RetType, IndexType>
    {
        private Proc<IndexType, RetType> setter;
        
        public PropertyIndexerSetter(Proc<IndexType, RetType> setter)
        {
            this.setter = setter;
        }

        public RetType this[IndexType index]
        {
            set { setter(index, value); }
        }
    }
}
