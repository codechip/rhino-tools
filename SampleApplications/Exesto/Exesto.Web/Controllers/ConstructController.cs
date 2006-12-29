using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.MonoRail.Framework;
using Exesto.Model;
using log4net;
using log4net.Appender;
using log4net.Config;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	public class ConstructController : Controller
	{
		public void Database()
		{
			MemoryAppender memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(LogManager.GetLogger(typeof (SchemaExport)).Logger.Repository, memoryAppender);
			ActiveRecordStarter.CreateSchema();
			PropertyBag["events"] = memoryAppender.GetEvents();
		}

		public void DummyData()
		{
			string[] names = {
				"accelerator", 
				"worm", 
				"Wittgenstein",
				"unprimed", 
			};
			DateTime start = DateTime.Now;
			With.Transaction(delegate
			{
				List<Subject> subjects = new List<Subject>();
				foreach (string name in names)
				{
					Subject subject = new Subject();
					subject.Name = name;
					subjects.Add(subject);
					Repository<Subject>.Save(subject);

					for (int i = 0; i < 50; i++)
					{
						Question q = new Question();
						q.Subject = subject;
						subject.Questions.Add(q);
						q.Title = "Question #" + i;
						q.Content = "Why?";
						q.Answer = "Because!";
						Repository<Question>.Save(q);
					}
				}
				PropertyBag["subjects"] = subjects;
			});
			PropertyBag["duration"] = DateTime.Now - start;
		}
	}
}