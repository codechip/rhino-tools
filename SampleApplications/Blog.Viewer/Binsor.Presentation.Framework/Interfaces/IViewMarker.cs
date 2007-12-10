namespace Binsor.Presentation.Framework.Interfaces
{
	/// <summary>
	/// This is useful as a marker interface, for the cases
	/// where you have behavior less view (consider a static header).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IViewMarker<T> : IView
		where T : IView
	{
	}
}