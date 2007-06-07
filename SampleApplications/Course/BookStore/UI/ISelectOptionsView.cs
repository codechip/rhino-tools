using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public interface ISelectOptionsView : IView
    {
        void AddCommand(string commandName, Command cmd);
        void Display();
    }
}
