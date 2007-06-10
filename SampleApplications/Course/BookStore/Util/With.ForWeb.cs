using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BookStore.Properties;
using System.Web;

namespace BookStore.Util
{
    public static class With
    {
        private static int TransactionCounter
        {
            get { return (int)(HttpContext.Current.Items[TransactionCounterKey] ?? 0); }
            set { HttpContext.Current.Items[TransactionCounterKey] = value; }
        }

        public static int TotalOpenedTransactionCount;

        public delegate T Func<T>(SqlCommand command);
        public delegate void Proc(SqlCommand command);

        static object TransactionCounterKey = new object();
        private static object ConnectionKey = new object();
        private static object TransactionKey = new object();

        private static SqlConnection ActiveConnection
        {
            get { return (SqlConnection)HttpContext.Current.Items[ConnectionKey]; }
            set { HttpContext.Current.Items[ConnectionKey] = value; }
        }
        private static SqlTransaction ActiveTransaction
        {
            get { return (SqlTransaction)HttpContext.Current.Items[TransactionKey]; }
            set { HttpContext.Current.Items[TransactionKey] = value; }
        }

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
                using (SqlCommand command = ActiveConnection.CreateCommand())
                {
                    command.Transaction = ActiveTransaction;
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
            if (TransactionCounter <= 0)
            {
                ActiveConnection.Dispose();
                ActiveConnection = null;
            }
        }

        private static void RollbackTransaction()
        {
            ActiveTransaction.Rollback();
            ActiveTransaction.Dispose();
            ActiveTransaction = null;
            TransactionCounter = 0;
        }

        private static void CommitTransaction()
        {
            TransactionCounter--;
            if (TransactionCounter == 0 && ActiveTransaction != null)
            {
                ActiveTransaction.Commit();
                ActiveTransaction.Dispose();
                ActiveTransaction = null;
            }
        }

        private static void StartTransaction(IsolationLevel isolation)
        {
            if (TransactionCounter <= 0)
            {
                TransactionCounter = 0;
                ActiveConnection = new SqlConnection(Settings.Default.Database);
                ActiveConnection.Open();
                ActiveTransaction = ActiveConnection.BeginTransaction(isolation);
                TotalOpenedTransactionCount++;
            }
            TransactionCounter++;
        }
    }
}
