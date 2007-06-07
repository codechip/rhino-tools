using System;
using System.Collections.Generic;
using System.Text;
using BookStore.CmdUI;
using BookStore.UI;
using System.Data;
using BookStore.Domain;


namespace BookStore.Controllers
{
    public class AddBookCopyController : BaseController<IAddBookCopyView>
    {
        readonly IAddBookCopyView view = ApplicationFactory.CreateAddBookCopyView();

        public override IAddBookCopyView View
        {
            get { return view; }
        }

        protected override void DoRun()
        {
            try
            {
                view.GetDataFromUser();
                Book book = ApplicationFactory.GetLibrary()
                    .GetBookByISBN(view.BookISBN);
                if (book == null)
                {
                    View.ShowMessage("Could not find book " + view.BookISBN);
                    return;
                }
                ApplicationFactory.GetLibrary().AddBookCopy(book);
                view.ShowMessage("Successfully created book: " + book.Name);
            }
            catch (DuplicateNameException e)
            {
                view.ShowError("Could not create book because the isbn " + view.BookISBN + " is not unique", e);
            }
            catch (Exception e)
            {
                view.ShowError("Could not create book because: " + e.Message, e);
            }

        }


    }
}
