using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public interface IAddBookView : IView
    {
        string BookName { get; }
        string BookISBN { get; }
        void GetDataFromUser();
    }
}
