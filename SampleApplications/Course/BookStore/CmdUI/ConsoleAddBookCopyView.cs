using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.Domain;
using BookStore.CmdUI;
using BookStore.Controllers;

namespace BookStore.CmdUI
{
    public class ConsoleAddBookCopyView : CommonConsoleView, IAddBookCopyView
    {
        string bookISBN;

        public void GetDataFromUser()
        {
            Console.Write("ISBN: ");
            bookISBN = Console.ReadLine();
        }

        public string BookISBN
        {
            get { return bookISBN; }
        }
    }
}
