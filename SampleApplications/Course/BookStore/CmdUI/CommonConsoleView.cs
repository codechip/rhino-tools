using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;

namespace BookStore.CmdUI
{
    public class CommonConsoleView : IView
    {
        public void ShowError(string message, Exception e)
        {
            using (Styles.Error)
            {
                Console.WriteLine(message);
                using (Styles.Important)
                {
                    Console.WriteLine("Would you like to see the full exception?");
                }
                if (Console.ReadKey().KeyChar == 'y')
                {
                    Console.WriteLine();
                    Console.WriteLine(e);
                }
            }
        }

        public void ShowMessage(string message)
        {
            using (Styles.Message)
            {
                Console.WriteLine(message);
            }
        }
    }
}
