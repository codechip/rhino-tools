using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.Domain;
using BookStore.CmdUI;

namespace BookStore.CmdUI
{
    public class ConsoleAddBookView : CommonConsoleView, IAddBookView
    {
        string bookName;
        string bookISBN;


        public void GetDataFromUser()
        {
            Console.Write("Book name: ");
            bookName = Console.ReadLine();
            Console.Write("ISBN: ");
            bookISBN = Console.ReadLine();
        }


        public string BookISBN
        {
            get { return bookISBN; }
        }

        public string BookName
        {
            get { return bookName; }
        }
    }
}
