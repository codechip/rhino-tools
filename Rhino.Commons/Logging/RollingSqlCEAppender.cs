using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using Rhino.Commons.Helpers;

namespace Rhino.Commons.Logging
{
	public class RollingSqlCEAppender : RollingEmbeddedDatabaseAppender
	{

        private string createScript =
            @"CREATE TABLE Logs (
	LogId		INT PRIMARY KEY IDENTITY(1,1),
	Date		DATETIME NOT NULL,
	Thread		NVARCHAR(255) NOT NULL,
	Level		NVARCHAR(50) NOT NULL,
	Logger		NVARCHAR(512) NOT NULL,
	Message		NTEXT NOT NULL,
	Exception	NTEXT NULL	
);";

        public RollingSqlCEAppender()
        {
            ConnectionType = "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe";
        }

	    public override string CreateScript
	    {
	        get { return createScript; }
	        set { createScript = value; }
	    }

	    protected override void CreateDatabaseFile()
	    {
	        SqlCEDbHelper.CreateDatabaseFile(CurrentFilePath);
	    }
	}
}