using System;
using System.Collections.Generic;
using System.Text;
using BookStore.Repositories;

namespace BookStore.Domain
{
    public class Library
    {
        //********************** Fields ***************************//
        //*** Books ***//

        BookRepository booksRepository = new BookRepository();
        ICollection<Book> books = new List<Book>();

        //*** Users ***//

        UsersRepository usersRepository = new UsersRepository();
        ICollection<User> users = new List<User>();


        //*********************** Methods **************************//
        //*** Books ***//
        
        public ICollection<Book> GetBooks()
        {
            return books;
        }

        public void AddBook(Book book)
        {
            booksRepository.Create(book);
            books.Add(book);
        }

        public int GetNumberOfCopies(Book book)
        {
            return booksRepository.GetCopies(book);
        }

        public void AddBookCopy(Book book)
        {
            booksRepository.AddBookCopy(book);
        }

        public IList<Book> GetBookList()
        {
            return booksRepository.GetBookList();
        }

        public IList<BookCopy> GetBookCopies(Book book1)
        {
            return booksRepository.GetBookCopies(book1);
        }

        public Book GetBookByISBN(string isbn)
        {
            return booksRepository.GetBookByISBN(isbn);
        }


        //*** Users ***//

        public void AddUser(User user)
        {
            usersRepository.Create(user);
            users.Add(user);
        }

        public IList<User> GetUsers()
        {
            return usersRepository.GetUsers();
        }

        public void CheckOutCopyOf(Book book1, User user)
        {
            // remember transactions!
            // find if book has free copies
            //  if not, throw NoFreeBookException
            // find copy id #
            // add new row to the checkouts table, with the proper data


            //booksRepository.CheckOut(book1, user, TimeSpan.FromDays(14) );
        }

        public ICollection<Book> GetBooksCheckedOutBy(User user)
        {
            return booksRepository.GetBooksCheckedOutBy(user);
        }
    }
}
