using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Domain
{
    public class LateReturn
    {
        User user;
        Book book;
        DateTime dueDate;
        DateTime checkedOutAt;

        public DateTime CheckedOutAt
        {
            get { return checkedOutAt; }
            set { checkedOutAt = value; }
        }

        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }

        public Book Book
        {
            get { return book; }
            set { book = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }

    }
}
