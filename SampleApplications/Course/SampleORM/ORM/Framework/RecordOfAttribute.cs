using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RecordOfAttribute : Attribute
    {
        string tableName;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public RecordOfAttribute(string tableName)
        {
            this.tableName = tableName;
        }
    }
}
