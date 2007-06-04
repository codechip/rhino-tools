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

        internal IList<Book> GetBookList()
        {
            return booksRepository.GetBookList();
        }

        internal IList<BookCopy> GetBookCopies(Book book1)
        {
            return booksRepository.GetBookCopies(book1);
        }

        internal Book GetBookByISBN(string isbn)
        {
            return booksRepository.GetBookByISBN(isbn);
        }


        //*** Users ***//

        public void AddUser(User user)
        {
            usersRepository.Create(user);
            users.Add(user);
        }

        internal IList<User> GetUsers()
        {
            return usersRepository.GetUsers();
        }

        internal void CheckOutCopyOf(Book book1, User user)
        {
            booksRepository.CheckOut(book1, user, TimeSpan.FromDays(14) );
        }

        internal ICollection<Book> GetBooksCheckedOutBy(User user)
        {
            return booksRepository.GetBooksCheckedOutBy(user);
        }
    }
}
