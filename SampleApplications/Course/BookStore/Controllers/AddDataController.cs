using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.CmdUI;
using BookStore.Controllers;

namespace BookStore.Controllers
{
    public class AddDataController : BaseMenuController
    {
        public AddDataController()
        {
            View.AddCommand("Add User", MoveToAddUserController);
            View.AddCommand("Add Book", MoveToAddBookController);
            View.AddCommand("Add Copy Of Existing Book", MoveToAddBookCopyController);
        }

        public void MoveToAddUserController()
        {
            new AddUserController().Run();
        }

        public void MoveToAddBookController()
        {
            new AddBookController().Run();
        }

        public void MoveToAddBookCopyController()
        {
            new AddBookCopyController().Run();
        }

    }

}
