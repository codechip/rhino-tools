namespace IoC.UI.Impl
{
	using System;
	using System.Collections;
	using System.Windows.Forms;
	using Interfaces;

	[Component("ApplicationContext")]
	public class DefaultApplicationContext : IApplicationContext
	{
		private readonly IShellView shell;
		private readonly IModuleLoader[] loaders;

		public DefaultApplicationContext(
			IShellView shell,
			IModuleLoader[] loaders
			)
		{
			this.shell = shell;
			this.loaders = loaders;
		}

		public void Start()
		{
			foreach (IModuleLoader loader in loaders)
			{
				loader.Initialize(this, shell);
			}

			Form form = GetShellAsForm();
			RunForm(form);
		}

		public virtual void RunForm(Form form)
		{
			Application.Run(form);
		}

		public virtual Form GetShellAsForm()
		{
			// not a really good solution, admittedly, but I can't think
			// of a strongly typed way of doing it. 
			Form form = shell as Form;
			if (form == null)
			{
				object type = shell == null ? (object)"'null'" : shell.GetType();
				throw new InvalidOperationException("The IShellView implementation " + type +
													" should be derived from " + typeof(Form));
			}
			return form;
		}
	}
}