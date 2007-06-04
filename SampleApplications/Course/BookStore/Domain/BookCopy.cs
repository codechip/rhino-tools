using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Domain
{
    public class BookCopy
    {
        string isbn;
        int id;

        public BookCopy(string isbn)
        {
            this.isbn = isbn;
        }
        
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string ISBN
        {
            get { return isbn; }
            set { isbn = value; }
        }
    }

  
}
