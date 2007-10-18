using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.Framework
{
    public class PersistedAttribute : Attribute
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
