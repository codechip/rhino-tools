using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Domain
{
    public class User
    {
        private  int id = 0;
        private string lastname;
        private string username;
        private string firstname;

        public User(string uname, string fname, string lname)
        {
            Username = uname;
            Firstname = fname;
            Lastname = lname;
        }

        public User()
        {

        }
        
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }

        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }
    }
}
                