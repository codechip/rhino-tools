using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Exesto.Model;
using Query;
using Rhino.Commons;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	public class FaqController : SmartDispatcherController
	{
		public void Index()
		{
				
		
			PropertyBag["subjects"] = PaginationHelper.CreatePagination(this, 
				Repository<Subject>.FindAll(),2);
		}

		public void ShowQuestions(int id)
		{
			Subject subject = Repository<Subject>.Load(id);
			ICollection<Question> questions = Repository<Question>.FindAll(
				Where.Question.Subject == subject);
			PropertyBag["subject"] = subject;
			PropertyBag["questions"] = PaginationHelper.CreatePagination(this,
			                                                             questions, 2);
		}
	}
}
