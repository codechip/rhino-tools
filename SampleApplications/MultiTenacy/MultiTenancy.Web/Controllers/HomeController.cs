using System.Web.Mvc;
using MultiTenancy.Web.Model;
using MultiTenancy.Web.Services;
using MultiTenancy.Web.ViewModel;
using NHibernate;

namespace MultiTenancy.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private readonly ISessionFactory sessionFactory;
        private readonly IScoreCalculator calculator;

        public HomeController(ISessionFactory sessionFactory, IScoreCalculator calculator)
        {
            this.sessionFactory = sessionFactory;
            this.calculator = calculator;
        }

        public ActionResult Index()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var scores = session.CreateCriteria(typeof(Score))
                    .List<Score>();

                var games = session.CreateCriteria(typeof (Game))
                    .SetMaxResults(5)
                    .List<Game>();

                var players = session.CreateCriteria(typeof (Player))
                    .SetMaxResults(5)
                    .List<Player>();

                return View(new IndexModel
                {
                    Calculator = calculator,
                    Players = players,
                    Games = games
                });
            }
        }
    }
}