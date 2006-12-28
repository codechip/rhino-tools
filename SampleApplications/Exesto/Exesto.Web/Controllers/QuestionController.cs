using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Exesto.Model;
using Rhino.Commons;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	[Scaffolding(typeof(Question))]
	public class QuestionController : Controller
	{
		public void New()
		{
			PropertyBag["subjects"] = Repository<Subject>.FindAll();
		}
	}
}
