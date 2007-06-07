using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.Domain;
using System.Data;

namespace BookStore.Controllers
{
    public class AddUserController : BaseController<IAddUserView>
    {
        readonly IAddUserView view = ApplicationFactory.CreateAddUserView();

        public override IAddUserView View
        {
            get { return view; }
        }

        protected override void DoRun()
        {
            view.GetDataFromUser();
            try
            {
                User user = new User(view.UserName, view.UserFirstName, view.UserLastName);
                ApplicationFactory.GetLibrary().AddUser(user);
            }
            catch (DuplicateNameException e)
            {
                view.ShowError("Could not create user because the name " + view.UserName + " is not unique",e);
                return;
            }
            catch (Exception e)
            {
                view.ShowError("Could not create user because: " + e.Message, e);
                return;
            }

            view.ShowMessage("Successfully created " + view.UserName);
        }

    }
}
