using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Domain
{
    public class User
    {
        private  int _ID = 0;
        private string _lastname;
        private string _username;
        private string _firstname;

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
            get { return _ID; }
            set { _ID = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }
    }
}
                