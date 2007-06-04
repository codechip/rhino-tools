using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BookStore.Domain;
using BookStore.Repositories;
using System.Data.SqlClient;
using BookStore.Properties;
using System.Data;

namespace BookStore.Tests
{
    [TestFixture]
    public class UsersTest : BaseTest
    {
        [Test]
        public void Create_User_And_Save()
        {
            Library library = new Library();
            User user = new User("Johny", "John", "Smith");
            library.AddUser(user);
            IList<User> users = library.GetUsers();
            Assert.AreEqual(1, users.Count, "Did not get a new user into the Library");

        }

        [Test]
        public void Can_Get_List_Of_Users()
        {
            Library library = new Library();

            library.AddUser(new User("Johny", "John", "Smith"));
            library.AddUser(new User("Assafgut", "Assaf", "Guterman"));
            library.AddUser(new User("Galitush", "Galit", "Guterman"));

            IList<User> users = library.GetUsers();
            Assert.AreEqual(3, users.Count);
        }

        [Test]
        [ExpectedException(typeof(DuplicateNameException),
            ExpectedMessage = "Username 'Johny' already exists")]
        public void If_Try_To_Insert_Duplicate_Name_Will_RaiseError()
        {
            Library library = new Library();

            library.AddUser(new User("Johny", "John", "Smith"));
            library.AddUser(new User("Johny", "John", "Smith"));
        }



    }
}
