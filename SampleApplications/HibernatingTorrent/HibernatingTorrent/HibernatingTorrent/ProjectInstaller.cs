using System.ComponentModel;
using System.Configuration.Install;

namespace HibernatingTorrent
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}
	}
}