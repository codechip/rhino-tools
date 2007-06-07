using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.CmdUI;
using BookStore.Domain;
using BookStore.WinUI;

namespace BookStore.Controllers
{
    public static class ApplicationFactory
    {
        public static bool IsOnConsole;

        static readonly Library library = new Library();

        public static ISelectOptionsView CreateSelectOptionsView()
        {
            if (IsOnConsole)
                return new ConsoleSelectOptionsView();
            else
                return new WinFormGenericUserInterface(); }

        public static Library GetLibrary()
        {
            return library;
        }

        public static IAddUserView CreateAddUserView()
        {
            if (IsOnConsole)
                return new ConsoleAddUserView();
            else
                throw new NotSupportedException("don't have view for this");
        
        }

        public static IAddBookView CreateAddBookView()
        {
            if (IsOnConsole)
                return new ConsoleAddBookView();
            else
                throw new NotSupportedException("don't have view for this");

        }

        public static IAddBookCopyView CreateAddBookCopyView()
        {
            if (IsOnConsole)
                return new ConsoleAddBookCopyView();
            else
                throw new NotSupportedException("don't have view for this");

        }
    }
}
