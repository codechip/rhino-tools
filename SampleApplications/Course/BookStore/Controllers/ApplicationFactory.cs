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

        static Library library = new Library();

        public static IGenericUserInterface CreateUserInterface()
        {
            if (IsOnConsole)
                return new GenericConsoleUserInterface();
            else
                return new WinFormGenericUserInterface(); }

        public static Library GetLibrary()
        {
            return library;
        }

        internal static IAddUserView CreateAddUserView()
        {
            if (IsOnConsole)
                return new ConsoleAddUserInterface();
            else
                throw new NotSupportedException("don't have view for this");
        
        }
    }
}
