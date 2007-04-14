namespace Rhino.Igloo
{
	/// <summary>
	/// Provide access to the current context
	/// </summary>
	public interface IContextProvider
	{
		/// <summary>
		/// Get the current context
		/// </summary>
		IContext Current { get; }
	}
}