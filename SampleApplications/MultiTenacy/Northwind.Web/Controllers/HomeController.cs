using System.Web.Mvc;
using MultiTenancy.Web.Model;
using MultiTenancy.Web.ViewModel;

namespace Northwind.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new IndexModel
            {
                Games = new Game[0],
                Players = new Player[0]
            });
        }
    }
}