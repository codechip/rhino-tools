using System;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface;
using NHibernate.Cfg;
using SLSExceptionReporter;
using System.Reflection;

namespace Ayende.NHibernateQueryAnalyzer
{
	/// <summary>
	/// Summary description for UserInterfaceMain.
	/// </summary>
	public class UserInterfaceMain
	{
		[STAThread()]
		public static void Main(string[] args)
		{
			try
			{
				Application.ThreadException+=new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
				Configuration cfg = new Configuration().AddAssembly(typeof(ProjectsRepository).Assembly);
				IProjectsRepository dataManager = new ProjectsRepository(cfg);
				Application.Run(new MainForm(dataManager));
			}
			catch (Exception e)
			{
				ShowError(e);
			}
		}

		private static void ShowError(Exception e)
		{
			ExceptionReporter reporter = new ExceptionReporter();
			reporter.ContactEmail = "Bugs@ayende.com";
			reporter.ContactWeb = "http://www.ayende.com/projects/nhibernate-query-analyzer.aspx";
			reporter.ContactPhone = "none, use email";
			reporter.ContactFax = "none, use email";
			reporter.ContactMessageTop = "An error occured that needs to be reported.";
			reporter.DisplayException(e);
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ShowError(e.Exception);
		}
	}
}