using System;
using System.Collections.Generic;
using System.Text;
using BookStore.Domain;
using BookStore.Util;
using System.Data.SqlClient;
using System.Data;

namespace BookStore.Repositories
{
    class UsersRepository
    {
        const string InsertUserSp = @"UserInsert";

        public void Create(User user)
        {
            try
            {
                With.Transaction(delegate(SqlCommand command)
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = InsertUserSp;
                    command.Parameters.AddWithValue("username", user.Username);
                    command.Parameters.AddWithValue("firstname", user.Firstname);
                    command.Parameters.AddWithValue("lastname", user.Lastname);
                    user.ID = Convert.ToInt32(command.ExecuteScalar());
                });
            }
            catch (SqlException e)
            {
                switch (e.Number)
                {
                    case 2627:
                        throw new DuplicateNameException("Username '" + user.Username + "' already exists", e);
                }
                throw;
            }
        }

        public IList<User> GetUsers()
        {
            return With.Transaction<IList<User>>(delegate(SqlCommand command)
                {
                    IList<User> users = new List<User>();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT Username, Firstname, Lastname,Id FROM Users";
                    using (SqlDataReader sdr = command.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            User user = new User();
                            user.Username = sdr.GetString(0);
                            user.Firstname = sdr.GetString(1);
                            user.Lastname = sdr.GetString(2);
                            user.ID = sdr.GetInt32(3);
                            users.Add(user);
                        }
                    }
                    return users;
                });
        }

    }


}
