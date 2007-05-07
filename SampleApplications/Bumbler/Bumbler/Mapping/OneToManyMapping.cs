using System.Text;
using Castle.ActiveRecord.Framework.Internal;

namespace Bumbler.Mapping
{
    public class OneToManyMapping
    {
        private string memberName;
        private ITable table;
        private IColumn column;

        public OneToManyMapping(string memberName,ITable table,IColumn column)
        {
            this.memberName = memberName;
            this.table = table;
            this.column = column;
        }
        public override string ToString()
        {
            StringBuilder mapping = new StringBuilder();
            mapping.AppendFormat("<bag name='{0}' inverse='true' lazy='true'>",
                                memberName).AppendLine();
            mapping.AppendFormat("<key column='{0}' />", column.Name).AppendLine();
            mapping.AppendFormat("<one-to-many class='{0}'/>", Inflector.Singularize(table.Name) ?? table.Name).AppendLine();
            mapping.Append("</bag>").AppendLine();
            return mapping.ToString();
        }
    }
}