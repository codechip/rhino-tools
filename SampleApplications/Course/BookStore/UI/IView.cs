using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public interface IView
    {
        void ShowError(string message, Exception e);
        void ShowMessage(string message);
    }
}
