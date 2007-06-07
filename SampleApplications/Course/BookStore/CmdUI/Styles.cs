using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.CmdUI
{
    public static class Styles
    {
        public class ChangeConsoleColor : IDisposable
        {
            readonly ConsoleColor old;

            public ChangeConsoleColor(ConsoleColor newColor)
            {
                old = Console.ForegroundColor;
                Console.ForegroundColor = newColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = old;
            }
        }

        public static IDisposable Important
        {
            get
            {
                return new ChangeConsoleColor(ConsoleColor.Magenta);
            }
        }

        public static IDisposable Message
        {
            get
            {
                return new ChangeConsoleColor(ConsoleColor.Yellow);
            }
        }

        public static IDisposable Error
        {
            get
            {
                return new ChangeConsoleColor(ConsoleColor.Red);
            }
        }
    }
}
