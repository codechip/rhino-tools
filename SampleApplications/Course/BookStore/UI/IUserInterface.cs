using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public delegate void Command();

    public interface IGenericUserInterface : IView
    {
        void AddCommand(string commandName, Command cmd);
        void Display();
    }
}
