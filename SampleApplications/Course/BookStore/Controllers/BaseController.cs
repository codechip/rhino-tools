using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;

namespace BookStore.Controllers
{
    public abstract class BaseController
    {
        public abstract IView View { get; }

        public void NotImplemented()
        {
            View.ShowMessage("Left for the students, do your homework!");
        }
    }
}
