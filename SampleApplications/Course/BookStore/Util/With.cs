using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BookStore.Properties;

namespace BookStore.Util
{
    public static class With
    {
        private static int transactionCounter = 0;

        public static int TotalOpenedTransactionCount;

        public delegate T Func<T>(SqlCommand command);
        public delegate void Proc(SqlCommand command);

        private static SqlConnection activeConnection;
        private static SqlTransaction activeTransaction;

        public static T Transaction<T>(Func<T> exec)
        {
            T result = default(T);
            Transaction(delegate(SqlCommand command)
            {
                result = exec(command);
            });
            return result;
        }

        public static void Transaction(Proc exec)
        {
            Transaction(IsolationLevel.ReadCommitted, exec);
        }

        public static void Transaction(IsolationLevel isolation, Proc exec)
        {
            StartTransaction(isolation);
            try
            {
                using (SqlCommand command = activeConnection.CreateCommand())
                {
                    command.Transaction = activeTransaction;
                    exec(command);
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
        }

        private static void CommitTransaction()
        {
            transactionCounter--;
            if (transactionCounter == 0 && activeTransaction != null)
            {
                activeTransaction.Commit();
                activeTransaction.Dispose();
                activeTransaction = null;
            }
        }

        private static void StartTransaction(IsolationLevel isolation)
        {
            if (transactionCounter <= 0)
            {
                transactionCounter = 0;
                activeConnection = new SqlConnection(Settings.Default.Database);
                activeConnection.Open();
                activeTransaction = activeConnection.BeginTransaction(isolation);
                TotalOpenedTransactionCount++;
            }
            transactionCounter++;
        }
    }
}
