using Castle.ActiveRecord;
using Castle.MonoRail.Framework;
using log4net;
using log4net.Appender;
using log4net.Config;
using NHibernate.Tool.hbm2ddl;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	public class ConstructController : Controller
	{
		public void Database()
		{
			MemoryAppender memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(
				LogManager.GetLogger(typeof(SchemaExport)).Logger.Repository,
				memoryAppender
				);
			ActiveRecordStarter.CreateSchema();
			PropertyBag["events"] = memoryAppender.GetEvents();
		}
	}
}
