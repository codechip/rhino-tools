using BookStore.UI;

namespace BookStore.Controllers
{
    public class MainController : BaseMenuController
    {
        public MainController()
        {
            View.AddCommand("Add Data", MoveToAddDataController);
            View.AddCommand("Checkout a book", NotImplemented);
            View.AddCommand("Return Book", NotImplemented);
            View.AddCommand("Search Book", NotImplemented);
            View.AddCommand("Search User", NotImplemented);
            View.AddCommand("Reports", MoveToReportsMenuController);
        }

        public void MoveToReportsMenuController()
        {
            new ReportsMenuController().Run();
        }

        public void MoveToAddDataController()
        {
            new AddDataController().Run();
        }
    }
}