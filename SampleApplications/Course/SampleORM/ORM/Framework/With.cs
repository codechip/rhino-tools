using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ORM.Framework
{
    internal static class With
    {
        internal static string ConnectionString;
        private static int transactionCounter = 0;

        public static int TotalOpenedTransactionCount;

        public delegate T Func<T>(IDbCommand command);
        public delegate void Proc(IDbCommand command);

        private static SqlConnection activeConnection;
        private static SqlTransaction activeTransaction;

        public static T Transaction<T>(Func<T> exec)
        {
            T result = default(T);
            Transaction(delegate(IDbCommand command)
            {
                result = exec(command);
            });
            return result;
        }

        public static void Transaction(Proc exec)
        {
            StartTransaction();
            try
            {
                using (SqlCommand command = activeConnection.CreateCommand())
                {
                    command.Transaction = activeTransaction;
                    exec(command);
                    Console.WriteLine(command.CommandText);
                    foreach (IDataParameter param in command.Parameters)
                    {
                        Console.WriteLine("\t{0} => {1}", param.ParameterName, param.Value);
                    }
                    Console.WriteLine("---");
                }
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        private static void DisposeTransaction()
        {
            if (transactionCounter <= 0)
            {
                activeConnection.Dispose();
                activeConnection = null;
            }
        }

        private static void RollbackTransaction()
        {
            activeTransaction.Rollback();
            activeTransaction.Dispose();
            activeTransaction = null;
            transactionCounter = 0;
            Console.WriteLine("Rollback Transaction");
        }

        private static void CommitTransaction()
        {
            transactionCounter--;
            if (transactionCounter == 0 && activeTransaction != null)
            {
                activeTransaction.Commit();
                activeTransaction.Dispose();
                activeTransaction = null;
                Console.WriteLine("Committed Transaction");
            }
        }

        private static void StartTransaction()
        {
            if (transactionCounter <= 0)
            {
                transactionCounter = 0;
                activeConnection = new SqlConnection(ConnectionString);
                activeConnection.Open();
                activeTransaction = activeConnection.BeginTransaction();
                TotalOpenedTransactionCount++;
                Console.WriteLine("Start Transaction");
            }
            transactionCounter++;
        }
    }
}
