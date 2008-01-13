namespace Rhino.Etl.Core.ConventionOperations
{
    using System;
    using System.Collections;
    using System.Data;
    using Operations;

    /// <summary>
    /// A convention based version of <see cref="OutputCommandOperation"/>. Will
    /// figure out as many things as it can on its own.
    /// </summary>
    public class ConventionOutputCommandOperation : OutputCommandOperation
    {
        private static Hashtable supportedTypes;
        private string command;


        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionOutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public ConventionOutputCommandOperation(string connectionStringName)
            : base(connectionStringName)
        {
        }

        /// <summary>
        /// Gets or sets the command to execute against the database
        /// </summary>
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        private static Hashtable SupportedTypes
        {
            get
            {
                if (supportedTypes == null)
                {
                    InitializeSupportedTypes();
                }
                return supportedTypes;
            }
        }

        private static void InitializeSupportedTypes()
        {
            supportedTypes = new Hashtable();
            supportedTypes[typeof (byte[])] = typeof (byte[]);
			supportedTypes[typeof (Guid)] = typeof (Guid);
            supportedTypes[typeof (Object)] = typeof (Object);
            supportedTypes[typeof (Boolean)] = typeof (Boolean);
            supportedTypes[typeof (SByte)] = typeof (SByte);
            supportedTypes[typeof (SByte)] = typeof (SByte);
            supportedTypes[typeof (Byte)] = typeof (Byte);
            supportedTypes[typeof (Int16)] = typeof (Int16);
            supportedTypes[typeof (UInt16)] = typeof (UInt16);
            supportedTypes[typeof (Int32)] = typeof (Int32);
            supportedTypes[typeof (UInt32)] = typeof (UInt32);
            supportedTypes[typeof (Int64)] = typeof (Int64);
            supportedTypes[typeof (UInt64)] = typeof (UInt64);
            supportedTypes[typeof (Single)] = typeof (Single);
            supportedTypes[typeof (Double)] = typeof (Double);
            supportedTypes[typeof (Decimal)] = typeof (Decimal);
            supportedTypes[typeof (DateTime)] = typeof (DateTime);
            supportedTypes[typeof (String)] = typeof (String);
        }

        /// <summary>
        /// Prepares the row by executing custom logic before passing on to the <see cref="PrepareCommand"/>
        /// for further process.
        /// </summary>
        /// <param name="row">The row.</param>
        protected virtual void PrepareRow(Row row)
        {
        }

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="row">The row.</param>
        protected override void PrepareCommand(IDbCommand cmd, Row row)
        {
            PrepareRow(row);
            cmd.CommandText = Command;
            foreach (string column in row.Columns)
            {
                object value = row[column];
                if (CanUseAsParameter(value))
                    AddParameter(column, value);
            }
        }

        /// <summary>
        /// Determines whether this value can be use as a parameter to ADO.Net provider.
        /// This perform a simple heuristic 
        /// </summary>
        /// <param name="value">The value.</param>
        private static bool CanUseAsParameter(object value)
        {
            return SupportedTypes.ContainsKey(value.GetType());
        }
    }
}