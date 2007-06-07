using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public interface IAddBookCopyView : IView
    {
        string BookISBN { get; }

        void GetDataFromUser();
    }
}
