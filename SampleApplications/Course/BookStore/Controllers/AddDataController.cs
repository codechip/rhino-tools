using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.CmdUI;

namespace BookStore.Controllers
{
    public class AddDataController : BaseController
    {
        IGenericUserInterface view = ApplicationFactory.CreateUserInterface();

        public override IView View
        {
            get { return view; }
        }

        public AddDataController()
        {
            view.AddCommand("Add User", MoveToAddUserController);
            view.AddCommand("Add Book", NotImplemented);
            view.AddCommand("Add Copy Of Existing Book", NotImplemented);
        }

        public void Run()
        {
            view.Display();
        }

        public void MoveToAddUserController()
        {
            new AddUserController().Run();
        }
    }

}
