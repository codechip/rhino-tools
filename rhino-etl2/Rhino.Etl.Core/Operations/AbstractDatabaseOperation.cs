namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Collections;
    using Commons;

    /// <summary>
    /// Represent an operation that uses the database can occure during the ETL process
    /// </summary>
    public abstract class AbstractDatabaseOperation : AbstractOperation
    {
        private readonly string connectionStringName;

        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName
        {
            get { return connectionStringName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected AbstractDatabaseOperation(string connectionStringName)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(connectionStringName),
                                             "Connection string name must have a value");
            this.connectionStringName = connectionStringName;
        }
    }
}