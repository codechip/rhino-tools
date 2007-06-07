using BookStore.UI;

namespace BookStore.Controllers
{
    public class MainController : BaseController
    {
        readonly ISelectOptionsView view = ApplicationFactory.CreateSelectOptionsView();

        public override IView View
        {
            get { return view; }
        }

        public MainController()
        {
            view.AddCommand("Add Data", MoveToAddDataController);
            view.AddCommand("Checkout a book", NotImplemented);
            view.AddCommand("Return Book", NotImplemented);
            view.AddCommand("Search Book", NotImplemented);
            view.AddCommand("Search User", NotImplemented);
            view.AddCommand("Reports", NotImplemented);
        }

        public void MoveToAddDataController()
        {
            new AddDataController().Run();
        }

        public void Run()
        {
            view.Display();
        }
    }
}