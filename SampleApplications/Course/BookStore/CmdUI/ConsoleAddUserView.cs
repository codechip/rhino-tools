using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;
using BookStore.Controllers;

namespace BookStore.CmdUI
{

    public class ConsoleAddUserView : CommonConsoleView, IAddUserView
    {
       string userLastName;
       string userFirstName;
       string userName;
        

        void IAddUserView.GetDataFromUser()
        {
            Console.Write("User first name: ");
            userFirstName = Console.ReadLine();
            Console.Write("User last name: ");
            userLastName = Console.ReadLine();
            Console.Write("User name: ");
            userName = Console.ReadLine();
        }


        string IAddUserView.UserLastName
        {
            get { return userLastName; }
        }

        string IAddUserView.UserFirstName
        {
            get { return userFirstName; }
        }

        string IAddUserView.UserName
        {
            get { return userName; }
        }
    }
}
