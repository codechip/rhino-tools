using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BookStore.Util;
using System.Data.SqlClient;

namespace BookStore.Tests
{
    public class BaseTest
    {
        [SetUp]
        public void SetupTests()
        {
            With.Transaction(delegate(SqlCommand command)
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "DELETE FROM CheckOuts";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM BookCopy";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM Books";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM Users";
                command.ExecuteNonQuery();

            });
        }
    }
}
