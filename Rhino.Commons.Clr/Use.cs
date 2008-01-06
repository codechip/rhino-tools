namespace Rhino.Commons
{
    using System;
    using System.Configuration;
    using System.Data;

    /// <summary>
    /// Helper class to provide simple data access, when we want to access the ADO.Net
    /// library directly. 
    /// </summary>
    public static class Use
    {
        #region Delegates

        public delegate T Func<T>(IDbCommand command);

        public delegate void Proc(IDbCommand command);

        #endregion

        private static readonly object activeConnectionKey = new object();
        private static readonly object activeTransactionKey = new object();
        private static readonly object transactionCounterKey = new object();

        private static IDbConnection ActiveConnection
        {
            get { return (IDbConnection) Local.Data[activeConnectionKey]; }
            set { Local.Data[activeConnectionKey] = value; }
        }

        private static IDbTransaction ActiveTransaction
        {
            get { return (IDbTransaction) Local.Data[activeTransactionKey]; }
            set { Local.Data[activeTransactionKey] = value; }
        }

        private static int TransactionCounter
        {
            get { return (int) (Local.Data[transactionCounterKey] ?? 0); }
            set { Local.Data[transactionCounterKey] = value; }
        }

        public static T Transaction<T>(string name, Func<T> exec)
        {
            T result = default(T);
            Transaction(name, delegate(IDbCommand command) { result = exec(command); });
            return result;
        }

        public static void Transaction(string name, Proc exec)
        {
            Transaction(name, IsolationLevel.Unspecified, exec);
        }

        public static void Transaction(string name, IsolationLevel isolation, Proc exec)
        {
            StartTransaction(name, isolation);
            try
            {
                using (IDbCommand command = ActiveConnection.CreateCommand())
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

        private static void StartTransaction(string name, IsolationLevel isolation)
        {
            if (TransactionCounter <= 0)
            {
                TransactionCounter = 0;
                ActiveConnection = Connection(name);
                ActiveTransaction = ActiveConnection.BeginTransaction(isolation);
            }
            TransactionCounter++;
        }

        /// <summary>
        /// Creates an open connection for a given named connection string, using the provider name
        /// to select the proper implementation
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The open connection</returns>
        public static IDbConnection Connection(string name)
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString == null)
                throw new InvalidOperationException("Could not find connnection string: " + name);
            Type type = Type.GetType(connectionString.ProviderName);
            if (type == null)
                throw new InvalidOperationException("The type name '" + connectionString.ProviderName +
                                                    "' could not be found for connection string: " + name);
            IDbConnection connection = (IDbConnection) Activator.CreateInstance(type);
            connection.ConnectionString = connectionString.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}