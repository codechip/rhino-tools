namespace IoC.UI.Interfaces
{
	public interface IModuleLoader
	{
		void Initialize(IApplicationContext context, IApplicationShell shell);
	}
}