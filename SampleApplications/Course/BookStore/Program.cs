using System;
using System.Collections.Generic;
using System.Text;
using BookStore.Repositories;
using BookStore.Domain;
using BookStore.Tests;
using BookStore.CmdUI;
using BookStore.Controllers;

namespace BookStore
{
    class Program
    {
        static void Main(string[] args)
        {
                ApplicationFactory.IsOnConsole = true;

                Console.WriteLine("Welcome to the library!");
                MainController controller = new MainController();
                controller.Run();
         
        }
    }
}
