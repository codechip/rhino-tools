using System;
using System.Web.Mvc;

namespace Eastsea.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            throw new IndexOutOfRangeException("n");
        }
    }
}