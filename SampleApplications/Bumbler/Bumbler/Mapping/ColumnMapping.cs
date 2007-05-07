using System;
using System.Text;
using Castle.ActiveRecord.Framework.Internal;

namespace Bumbler.Mapping
{
    public class ColumnMapping
    {
        private IColumn column;
        public string Name
        {
            get
            {
                return column.Name;
            }
        }
        public ColumnMapping(IColumn column)
        {
            this.column = column;
        }
        public override string ToString()
        {
            StringBuilder mapping=new StringBuilder();
            if (column.IsPK)
            {
                mapping.AppendFormat("<id name='{0}' type='{1}'>", column.Name, column.ClrTypeName)
                    .AppendLine();
                mapping.AppendLine("<generator class='native'/>");// probably not the best idea, but simplest
                mapping.AppendLine("</id>");
            }
            else if (column.IsFK)
            {
                mapping.AppendFormat("<many-to-one name='{2}' class='{1}' column='{0}' />",
                    column.Name, Inflector.Singularize(column.FkTableName) ?? column.FkTableName, column.Name.EndsWith("ID",StringComparison.InvariantCultureIgnoreCase) ? column.Name.Substring(0,column.Name.Length-2) : column.Name)
                    .AppendLine();
            }
            else
            {
                mapping.AppendFormat("<property name='{0}' type='{1}'/>", column.Name, column.ClrTypeName)
                    .AppendLine();
            }
            return mapping.ToString();
        }

    }
}