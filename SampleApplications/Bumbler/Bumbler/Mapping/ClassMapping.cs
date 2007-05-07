using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord.Framework.Internal;

namespace Bumbler.Mapping
{
    public class ClassMapping
    {
        private ITable table;
        private IList<ColumnMapping> columns = new List<ColumnMapping>();
        private IList<OneToManyMapping> bags = new List<OneToManyMapping>();

        public ClassMapping(ITable table, Dictionary<string, ClassMapping> maps)
        {
            this.table = table;
            foreach (IColumn column in table.Columns)
            {
                columns.Add(new ColumnMapping(column));
            }
        }

        public void AddOneToMany(OneToManyMapping mapping)
        {
            bags.Add(mapping);
        }
        public void CreateOneToManyMappings(Dictionary<string,ClassMapping> allMappings)
        {
            foreach (IColumn column in table.Columns)
            {
                if (column.IsFK)
                {
                    if (allMappings.ContainsKey(column.FkTableName))
                    {
                        if (!HasColumn(Inflector.Pluralize(table.Name)))
                        {
                            allMappings[column.FkTableName].AddOneToMany(new OneToManyMapping(Inflector.Pluralize(table.Name),table, column));
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder mapping = new StringBuilder();
            mapping.AppendLine("<?xml version=\"1.0\" ?>");
            mapping.AppendLine(
                "<hibernate-mapping namespace='Bumble' auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">");
            mapping.AppendFormat("<class name='{0}' table='{1}'>", Inflector.Singularize(table.Name) ?? table.Name, table.Name)
                .AppendLine();
            foreach(ColumnMapping columnMapping in columns)
            {
                mapping.Append(columnMapping.ToString());
            }
            foreach(OneToManyMapping bag in bags)
            {
                mapping.Append(bag.ToString());
            }
            mapping.AppendLine("</class>")
                .AppendLine("</hibernate-mapping>");
            return mapping.ToString();
        }
        private bool HasColumn(string name)
        {
            foreach(ColumnMapping columnMapping in columns)
            {
                if (columnMapping.Name == name) return true;
            }
            return false;
        }
    }
}