using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BookStore.Domain;
using System.Data.SqlClient;
using BookStore.Properties;
using BookStore.Util;

namespace BookStore.Tests
{
    [TestFixture]
    public class BooksTest : BaseTest
    {
        [Test]
        public void List_Of_Books_From_EmptyLibrary_Has_No_Books()
        {
            Library library = CreateTestLibrary();
            ICollection<Book> books = library.GetBooks();
            Assert.AreEqual(0, books.Count);
        }

        [Test]
        public void Can_Add_Book_To_Library()
        {
            Library library = CreateTestLibrary();
            library.AddBook(CreateBook());
            ICollection<Book> books = library.GetBooks();
            Assert.AreEqual(1, books.Count);
        }

        [Test]
        public void Book_Has_Name_Settable_From_Constructor()
        {
            Book book = CreateBook();
            Assert.AreEqual("War & Peace", book.Name);
        }

        [Test]
        public void Add_New_Book_Will_Create_A_Single_Copy()
        {
            Library library = CreateTestLibrary();
            Book book = CreateBook();

            library.AddBook(book);

            int copies = library.GetNumberOfCopies(book);

            Assert.AreEqual(1, copies);
        }



        private static Book CreateBook()
        {
            return new Book("1-1231-13", "War & Peace");
        }

        private static Library CreateTestLibrary()
        {
            return new Library();
        }

        [Test]
        public void Add_New_Copy_Of_Book()
        {
            Library library = CreateTestLibrary();
            Book book = CreateBook();

            library.AddBook(book);
            library.AddBookCopy(book);
            int copies = library.GetNumberOfCopies(book);

            Assert.AreEqual(2, copies);
        }

        [Test]
        public void Get_Book_List()
        {
            Library library = CreateTestLibrary();
            Book book1 = new Book("1-1231-13", "War & Peace vol 1");
            Book book2 = new Book("1-1231-14", "War & Peace vol 2");
            Book book3 = new Book("1-1231-15", "War & Peace vol 3");

            library.AddBook(book1);
            library.AddBook(book2);
            library.AddBook(book3);

            IList<Book> bl = library.GetBookList();


            Assert.AreEqual("War & Peace vol 2", bl[1].Name);
        }

        [Test]
        public void Get_Book_Copies()
        {
            Library library = CreateTestLibrary();
            Book book1 = new Book("1-1231-13", "War & Peace vol 1");

            library.AddBook(book1);
            library.AddBookCopy(book1);
            library.AddBookCopy(book1);

            IList<BookCopy> bcl = library.GetBookCopies(book1);

            Assert.AreEqual(3, bcl.Count);
        }

        [Test]
        public void Get_Book_By_ID()
        {
            Library library = CreateTestLibrary();
            Book book1 = new Book("1-1231-13", "War & Peace vol 1");
            Book book2;
            library.AddBook(book1);

            book2 = library.GetBookByISBN("1-1231-13");
            Assert.AreEqual(book1.Name, book2.Name);
        }

        [Test]
        public void Add_Book_And_Five_Copies_With_Single_Transaction()
        {
            With.TotalOpenedTransactionCount = 0;

            With.Transaction(delegate
            {
                Library library = CreateTestLibrary();
                Book book1 = new Book("1-1231-13", "War & Peace vol 1");
                library.AddBook(book1);
                for (int i = 0; i < 4; i++)
                    library.AddBookCopy(book1);

            });

            Assert.AreEqual(1, With.TotalOpenedTransactionCount);
        }

        [Test]
        public void Can_Check_A_Book_From_Library()
        {
            Library library = CreateTestLibrary();
            Book book1 = new Book("1-1231-13", "War & Peace vol 1");
            library.AddBook(book1);

            User user = new User("read-alot", "read", "a lot");
            library.AddUser(user);


            ICollection<Book> books = library.GetBooksCheckedOutBy(user);
            Assert.AreEqual(0, books.Count);

            library.CheckOutCopyOf(book1, user);

            books = library.GetBooksCheckedOutBy(user);
            Assert.AreEqual(1, books.Count);
        }
    }
}
