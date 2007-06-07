using System;
using System.Collections.Generic;
using System.Text;
using BookStore.Domain;
using BookStore.Properties;
using BookStore.Util;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace BookStore.Repositories
{
    public class BookRepository
    {
        const string InsertBookSp = @"BookInsert";
        const string BookGetAllCheckedOutByUser = "BookGetAllCheckedOutByUser";
        const string BookFindAvailableCopy = "BookFindAvailableCopy";

        public void Create(Book book)
        {
            With.Transaction(delegate(SqlCommand command)
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = InsertBookSp;
                command.Parameters.AddWithValue("isbn", book.ISBN);
                command.Parameters.AddWithValue("name", book.Name);
                command.ExecuteNonQuery();
            });
        }

        public int GetCopies(Book book)
        {
            return With.Transaction<int>(delegate(SqlCommand command)
              {
                  command.CommandType = System.Data.CommandType.Text;
                  command.CommandText = "SELECT COUNT(*) FROM BookCopy WHERE ISBN = @ISBN";
                  command.Parameters.AddWithValue("isbn", book.ISBN);
                  return (int)command.ExecuteScalar();
              });
        }

        public IList<Book> GetBookList()
        {
            return With.Transaction<IList<Book>>(delegate(SqlCommand command)
             {
                 command.CommandType = System.Data.CommandType.Text;
                 command.CommandText = "SELECT ISBN, Name FROM Books";
                 return GetBooksListFromCommand(command);
             });
        }

        private IList<Book> GetBooksListFromCommand(SqlCommand command)
        {
            IList<Book> mybooklist = new List<Book>();
            using (SqlDataReader mydr = command.ExecuteReader())
            {
                while (mydr.Read())
                {
                    Book mybook = new Book(mydr.GetString(0), mydr.GetString(1));
                    mybooklist.Add(mybook);
                }
            }
            return mybooklist;
        }

        public IList<BookCopy> GetBookCopies(Book mybook)
        {
            return With.Transaction<IList<BookCopy>>(delegate(SqlCommand command)
             {
                 IList<BookCopy> mybookcopylist = new List<BookCopy>();

                 command.CommandType = System.Data.CommandType.Text;
                 command.CommandText = "SELECT Id, ISBN FROM BookCopy where isbn=@isbn";
                 command.Parameters.AddWithValue("isbn", mybook.ISBN);
                 using (SqlDataReader mydr = command.ExecuteReader())
                 {
                     while (mydr.Read())
                     {
                         BookCopy mybookcopy = new BookCopy(mydr.GetString(1));
                         mybookcopy.Id = mydr.GetInt32(0);
                         mybookcopylist.Add(mybookcopy);
                     }
                 }
                 return mybookcopylist;
             });
        }

        public Book GetBookByISBN(String bookisbn)
        {
            return With.Transaction<Book>(delegate(SqlCommand command)
            {
                Book mybook;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT * FROM Books where isbn=@isbn";
                command.Parameters.AddWithValue("isbn", bookisbn);
                using (SqlDataReader mydr = command.ExecuteReader())
                {
                    if (mydr.HasRows)
                    {
                        mydr.Read();
                        mybook = new Book(mydr.GetString(0), mydr.GetString(1));
                        return mybook;
                    }
                    else
                    {
                        return null;
                    }
                }

            });
        }

        public void AddBookCopy(Book book)
        {
            With.Transaction(delegate(SqlCommand command)
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "insert into BookCopy (ISBN) values(@ISBN)";
                command.Parameters.AddWithValue("isbn", book.ISBN);
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// This is not working!
        /// Left for home
        /// </summary>
        public void CheckOut(Book book, User user, TimeSpan checkOutDuration)
        {
            With.Transaction(IsolationLevel.Serializable, delegate(SqlCommand command)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = BookFindAvailableCopy;
                command.Parameters.AddWithValue("isbn", book.ISBN);
                object value = command.ExecuteScalar();
                int copyId = (int)value;
                command.CommandType = CommandType.Text;
                command.CommandText = @"INSERT INTO CheckOuts(BookCopyId, UserId, CheckedOutAt, DueDate) 
                    VALUES(@copyId, @userId, @checkOutAt, @dueDate)";
                command.Parameters.AddWithValue("copyId", copyId);
                command.Parameters.AddWithValue("userId", user.ID);
                command.Parameters.AddWithValue("checkOutAt", DateTime.Today);
                command.Parameters.AddWithValue("dueDate", DateTime.Today.Add(checkOutDuration));
                command.ExecuteNonQuery();
            });
        }

        public ICollection<Book> GetBooksCheckedOutBy(User user)
        {
            return With.Transaction<ICollection<Book>>(delegate(SqlCommand command)
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "BookGetAllCheckedOutByUser";
                command.Parameters.AddWithValue("userid", user.ID);
                return GetBooksListFromCommand(command);
            });
        }
    }
}
