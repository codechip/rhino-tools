namespace IoC.UI
{
	using System.Windows.Forms;

	public static class Commands
	{
		public static void Dispatch(string name)
		{
			MessageBox.Show("Command invoked: " + name);
		}
	}
}