using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Domain
{
    public class Book
    {
        string iSBN;
        string name;

        public Book(string isbn, string name)
        {
            this.iSBN = isbn;
            this.name = name;
        }

        public string ISBN
        {
            get { return iSBN; }
            set { iSBN = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
