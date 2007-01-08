using Castle.MonoRail.Framework;
using Exesto.Model;
using Exesto.Web.Services;
using Rhino.Commons;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	[Scaffolding(typeof(Subject))]
	public class SubjectController : Controller
	{
		public void TestTransactions_NoThrow()
		{
			JustService justService = IoC.Resolve<JustService>();
			justService.Insert(false);
		}

		public void TestTransactions_Throw()
		{
			JustService justService = IoC.Resolve<JustService>();
			justService.Insert(true);
		}
	}
}
