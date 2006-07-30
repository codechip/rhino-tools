using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Rhino.Commons
{
    public class BulkDeleter<PkType, ItemType>
    {
        DataTable table;
        string tempTable, procName;
        Func<PkType, ItemType> getPk;
        Guid guid = Guid.NewGuid();
        
        public BulkDeleter(string tempTableName, string procName, Func<PkType, ItemType> getPk)
        {
            this.tempTable = tempTableName;
            this.procName = procName;
            this.table = new DataTable(tempTable);
            this.table.Columns.Add("Id", typeof(PkType)).Unique = true;
            this.table.Columns.Add("Guid", typeof(Guid));
            this.getPk = getPk;
        }

        public void RegisterForDeletion(ItemType item)
        {
            lock (table)
            {
                table.Rows.Add(getPk(item),guid);
            }
        }

        public void PerformDelete(IDbConnection connection)
        {
            lock (table)
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)connection))
                {
                    bulkCopy.DestinationTableName = tempTable;
                    bulkCopy.WriteToServer(table);
                }

                table.Rows.Clear();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procName;
                    command.CommandType = CommandType.StoredProcedure;
                    IDbDataParameter arg = command.CreateParameter();
                    arg.DbType = DbType.Guid;
                    arg.Value = guid;
                    arg.ParameterName = "guid";
                    command.Parameters.Add(arg);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
