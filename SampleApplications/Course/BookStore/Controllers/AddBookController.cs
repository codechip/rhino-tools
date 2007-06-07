using System;
using System.Collections.Generic;
using System.Text;
using BookStore.CmdUI;
using BookStore.Domain;
using BookStore.UI;
using System.Data;


namespace BookStore.Controllers
{
    public class AddBookController : BaseController<IAddBookView>
    {
        readonly IAddBookView view = ApplicationFactory.CreateAddBookView();

        public override IAddBookView View
        {
            get { return view; }
        }

        protected override void DoRun()
        {
            view.GetDataFromUser();
            try
            {
                Book book = new Book(view.BookISBN, view.BookName);
                ApplicationFactory.GetLibrary().AddBook(book);
            }
            catch (DuplicateNameException e)
            {
                view.ShowError("Could not create book because the isbn " + view.BookISBN + " is not unique", e);
                return;
            }
            catch (Exception e)
            {
                view.ShowError("Could not create book because: " + e.Message, e);
                return;
            }

            view.ShowMessage("Successfully created book" + view.BookName);
        }

    }
}
