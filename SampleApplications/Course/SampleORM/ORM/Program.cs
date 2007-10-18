using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ORM.Framework;

namespace ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=localhost\sqlexpress;Integrated Security=SSPI;Initial Catalog=Demo";
            Database.Init(typeof(User).Assembly, connectionString);

            User[] users = Database.FindAll<User>();
            //Database.Transaction(delegate
            //{
            //    foreach (User user in users)
            //    {
            //        user.Email = user.Firstname + "$" + user.Lastname + "@bar.com";
            //        Database.Save(user);
            //    }
            //});

            Database.Transaction(delegate
             {
                 User user = new User();
                 user.Lastname = "lala";
                 user.Firstname = "orennn";
                 user.Email = "orenla@yyy.com";
                 user.Username = "username";

                 Database.Create(user);
                 Console.WriteLine("User {0}, created with ID: {1}", user.Username, user.Id);
             });

            Database.Transaction(delegate
             {
                 User user = new User();
                 user.Lastname = "lala";
                 user.Firstname = "orennn";
                 user.Email = "orenla@yyy.com";
                 user.Username = "username";

                 Database.Create(user);
                 Console.WriteLine("User {0}, created with ID: {1}", user.Username, user.Id);
                 bool res=Database.Delete(user);
                 Console.WriteLine(res);  
             });
        }
    }
}
