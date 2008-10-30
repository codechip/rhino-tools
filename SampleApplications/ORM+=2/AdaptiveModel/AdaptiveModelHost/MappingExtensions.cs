using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Mapping;
using System.Collections.Generic;

namespace AdaptiveModelHost
{
    public static class MappingExtensions
    {
        public static void SetSchema(this Configuration cfg, string schema)
        {
            foreach (var persistentClass in cfg.ClassMappings)
            {
                persistentClass.Table.Schema = schema;
            }
        }

        public static void MapManyToOne<TEntityInterface, TEntity>(this Configuration cfg)
        {
            foreach (var persistentClass in cfg.ClassMappings)
            {
                var propertyNames = new List<string>();
                foreach (PropertyInfo property in persistentClass.MappedClass.GetProperties())
                {
                    if (property.PropertyType == typeof(TEntityInterface))
                    {
                        propertyNames.Add(property.Name);
                    }
                }
                if (propertyNames.Count == 0)
                    continue;

                var prop = new Property();
                PersistentClass targetClass = cfg.GetClassMapping(typeof(TEntity));

                foreach (string propertyName in propertyNames)
                {
                    Table table = targetClass.Table;
                    var value = new ManyToOne(table);
                    value.ReferencedEntityName = typeof(TEntity).FullName;
                    var column = new Column(propertyName);
                    value.AddColumn(column);
                    prop.Value = value;
                    prop.Name = propertyName;
                    prop.PersistentClass = targetClass;
                    persistentClass.AddProperty(prop);
                    persistentClass.Table.AddColumn(column);
                    string fkName = string.Format("FK_{0}To{1}", propertyName, persistentClass.MappedClass.Name);
                    persistentClass.Table.CreateForeignKey(fkName,
                                                           new[] { column, }, typeof(TEntity).FullName);
                }
            }
        }
    }
}