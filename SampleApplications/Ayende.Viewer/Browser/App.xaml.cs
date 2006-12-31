using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Model;

namespace Browser
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>

	public partial class App : System.Windows.Application
	{
		private SessionScope sessionScope;

		public App()
		{
			ActiveRecordStarter.Initialize(typeof(Post).Assembly, ActiveRecordSectionHandler.Instance);
			sessionScope = new SessionScope();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			sessionScope.Dispose();
			base.OnExit(e);
		}
	}
}