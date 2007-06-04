using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.UI
{
    public interface IAddUserView : IView
    {
        string UserLastName { get; }
        string UserFirstName { get; }
        string UserName { get; }
        void GetDataFromUser();
    }
}
