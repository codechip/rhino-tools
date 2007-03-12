using System;
using System.Data;

namespace Rhino.Commons.Logging
{
    public class RollingSqlliteAppender : RollingEmbeddedDatabaseAppender
    {

        private string createScript =
            @"CREATE TABLE Logs (
	LogId		INT PRIMARY KEY,
	Date		DATETIME NOT NULL,
	Thread		NVARCHAR(255) NOT NULL,
	Level		NVARCHAR(50) NOT NULL,
	Logger		NVARCHAR(512) NOT NULL,
	Message		NTEXT NOT NULL,
	Exception	NTEXT NULL	
);";
        
        public RollingSqlliteAppender()
        {
            ConnectionType = "System.Data.SQLite.SQLiteConnection, System.Data.SQLite";
        }

        public override string CreateScript
        {
            get { return createScript; }
            set { createScript = value; }
        }

        protected override void CreateDatabaseFile()
        {
            using (IDbConnection connection = (IDbConnection)Activator.CreateInstance(this.ResolveConnectionType()))
            {
                connection.ConnectionString = ConnectionString + "; New=True";
                connection.Open();
            }
        }
    }
}