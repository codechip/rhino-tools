using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace ORM.Framework
{
    public class Database
    {
        static List<Mapping> mappings = new List<Mapping>();

        public static void Init(Assembly assembly, string connectionString)
        {
            With.ConnectionString = connectionString;
            foreach (Type type in assembly.GetTypes())
            {
                RecordOfAttribute[] records =
                    (RecordOfAttribute[])type.GetCustomAttributes(typeof(RecordOfAttribute), true);
                if (records.Length == 0)
                    continue;
                mappings.Add(new Mapping(type, records[0]));
            }
        }

        public class Mapping
        {
            public Mapping(Type type, RecordOfAttribute recordOf)
            {
                this.Type = type;
                this.TableName = recordOf.TableName;
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                    {
                        this.PrimaryKey = new KeyValuePair<string, PropertyInfo>(property.Name, property);
                    }
                    else if (property.GetCustomAttributes(typeof(PersistedAttribute), true).Length > 0)
                    {
                        Columns.Add(new KeyValuePair<string, PropertyInfo>(property.Name, property));
                    }
                }
            }

            public Type Type;
            public string TableName;
            public KeyValuePair<string, PropertyInfo> PrimaryKey;
            public List<KeyValuePair<string, PropertyInfo>> Columns = new List<KeyValuePair<string, PropertyInfo>>();
        }

        public static TEntity[] FindAll<TEntity>() where TEntity : new()
        {
            return With.Transaction<TEntity[]>(delegate(IDbCommand command)
            {
                Mapping mapping = GetMappingFor(typeof(TEntity));
                command.CommandText = GenerateSelect<TEntity>();
                using (IDataReader reader = command.ExecuteReader())
                {
                    List<TEntity> entities = new List<TEntity>();
                    while (reader.Read())
                    {
                        TEntity entity = new TEntity();
                        object value = reader.GetValue(reader.GetOrdinal(mapping.PrimaryKey.Key));

                        mapping.PrimaryKey.Value.SetValue(entity,
                            Convert.ChangeType(value, mapping.PrimaryKey.Value.PropertyType), null);

                        foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
                        {
                            value = reader.GetValue(reader.GetOrdinal(kvp.Key));
                            if (value == DBNull.Value)
                                value = null;

                            kvp.Value.SetValue(entity,
                                Convert.ChangeType(value, kvp.Value.PropertyType), null);
                        }

                        entities.Add(entity);
                    }
                    return entities.ToArray();
                }
            });
        }

        private static string GenerateSelect<TEntity>()
        {
            Mapping mapping = GetMappingFor(typeof(TEntity));

            StringBuilder select = new StringBuilder();
            select.Append("SELECT ");
            select
                 .Append("[")
                 .Append(mapping.PrimaryKey.Key)
                 .Append("]");
            foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
            {
                select.Append(", ")
                    .Append("[")
                    .Append(kvp.Key)
                    .Append("]");
            }
            select.Append(" FROM ")
                 .Append("[")
                 .Append(mapping.TableName)
                 .Append("]");

            return select.ToString();
        }

        private static string GenerateUpdate<TEntity>()
        {
            Mapping mapping = GetMappingFor(typeof(TEntity));

            StringBuilder update = new StringBuilder();
            update.Append("UPDATE ")
                 .Append("[")
                 .Append(mapping.TableName)
                 .Append("]")
                 .Append(" SET ");
            int paramIndex = 0;
            foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
            {
                update
                    .Append("[")
                    .Append(kvp.Key)
                    .Append("]")
                    .Append(" = ")
                    .Append("@p").Append(paramIndex);
                paramIndex += 1;
                update.Append(", ");
            }
            update.Remove(update.Length - 2, 2);
            update.Append(" WHERE ")
                 .Append("[")
                 .Append(mapping.PrimaryKey.Key)
                 .Append("]")
                 .Append(" = ")
                .Append("@id");
            return update.ToString();
        }

        private static string GenerateCreate<TEntity>()
        {
            Mapping mapping = GetMappingFor(typeof(TEntity));

            StringBuilder insert = new StringBuilder();
            insert.Append("Insert Into ");
            insert
                 .Append("[")
                 .Append(mapping.TableName)
                 .Append("]");

            insert.Append(" (");
            foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
            {
                insert.Append("[")
                    .Append(kvp.Key)
                    .Append("]")
                    .Append(", ");
            }
            insert.Remove(insert.Length - 2, 2);
            insert.Append(") values (");

            int paramIndex = 0;
            foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
            {
                insert.Append("@p")
                    .Append(paramIndex)
                    .Append(", ");
                paramIndex += 1;
            }
            insert.Remove(insert.Length - 2, 2);
            insert.AppendLine(" );");
            insert.Append("Select scope_identity();");


            return insert.ToString();
        }

        private static Mapping GetMappingFor(Type type)
        {
            foreach (Mapping mapping in mappings)
            {
                if (mapping.Type == type)
                    return mapping;
            }
            throw new Exception("Type not found: " + type);
        }


        public static void Save<TEntity>(TEntity entity)
        {
            With.Transaction(delegate(IDbCommand command)
            {
                Mapping mapping = GetMappingFor(typeof(TEntity));

                command.CommandText = GenerateUpdate<TEntity>();
                object value = mapping.PrimaryKey.Value.GetValue(entity, null);
                AddParameter(command, "@id", value);
                int parameterIndex = 0;
                foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
                {
                    value = kvp.Value.GetValue(entity, null);
                    AddParameter(command, "@p" + parameterIndex, value);
                    parameterIndex += 1;
                }

                command.ExecuteNonQuery();
            });
        }

        private static void AddParameter(IDbCommand command, string parameterName, object value)
        {
            IDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }

        public delegate void Proc();

        public static void Transaction(Proc proc)
        {
            With.Transaction(delegate
            {
                proc();
            });
        }


        public static void Create<TEntity>(TEntity entity)
        {
            With.Transaction(delegate(IDbCommand command)
            {
                Mapping mapping = GetMappingFor(typeof(TEntity));

                command.CommandText = GenerateCreate<TEntity>();

                int parameterIndex = 0;
                foreach (KeyValuePair<string, PropertyInfo> kvp in mapping.Columns)
                {
                    object value = kvp.Value.GetValue(entity, null);
                    AddParameter(command, "@p" + parameterIndex, value);
                    parameterIndex += 1;
                }

                object obj = command.ExecuteScalar();
                mapping.PrimaryKey.Value.SetValue(entity,
                    Convert.ChangeType(obj, mapping.PrimaryKey.Value.PropertyType), null);
            });
        }

        public static bool Delete<TEntity>(TEntity entity)
        {
            return With.Transaction <bool>(delegate(IDbCommand command)
            {
                Mapping mapping = GetMappingFor(typeof(TEntity));

                command.CommandText = GenerateDelete<TEntity>();
                object value = mapping.PrimaryKey.Value.GetValue(entity, null);
                AddParameter(command, "@id", value);

                return command.ExecuteNonQuery() != 0;

            });
        }

        private static string GenerateDelete<TEntity>()
        {
            Mapping mapping = GetMappingFor(typeof(TEntity));

            StringBuilder delete = new StringBuilder();
            delete.Append("DELETE FROM ")
                 .Append("[")
                 .Append(mapping.TableName)
                 .Append("]")
                 .Append(" WHERE [")
                 .Append(mapping.PrimaryKey.Key)
                 .Append("] = @id");

            return delete.ToString();
        }

    }
}
