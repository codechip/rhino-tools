using System;
using System.Collections.Generic;
using System.Text;
using ORM.Framework;

namespace ORM
{
    [RecordOf("Users")]
    public class User
    {
        int id;
        string username;
        string firstname;
        string lastname;
        string email;

        [Persisted]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [Persisted]
        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }

        [Persisted]
        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }

        [Persisted]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [PrimaryKey]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
