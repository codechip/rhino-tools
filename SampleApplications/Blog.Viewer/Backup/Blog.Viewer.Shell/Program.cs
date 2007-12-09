namespace Blog.Viewer.Shell
{
	using System;
	using System.Windows.Forms;
	using IoC.UI.Interfaces;
	using Rhino.Commons;

	class Program
	{
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


			try
			{
				IoC.Initialize(new RhinoContainer("assembly://Blog.Viewer.Shell/Windsor.boo"));
				IApplicationContext applicationContext = IoC.Resolve<IApplicationContext>();
				applicationContext.Start();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex);
			}
		}
	}
}
