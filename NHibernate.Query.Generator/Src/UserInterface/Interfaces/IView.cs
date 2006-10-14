using System;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces
{
	public interface IView : IDisposable
	{
		/// <summary>
		/// Starts the wait message by the UI. The view need to check every <c>checkInterval</c> 
		/// that the work was comleted (using <c>HasFinishedWork()</c> method).
		/// The work should finish in shouldWaitFor, but there is no gurantee about it.
		/// <c>EndWait</c> is called to end the wait.
		/// </summary>
		/// <param name="waitMessage">The Wait message.</param>
		/// <param name="checkInterval">Check interval.</param>
		/// <param name="shouldWaitFor">Should wait for.</param>
		void StartWait(string waitMessage, int checkInterval, int shouldWaitFor);
		void EndWait(string endMessage);
		void AddException(Exception ex);
		void ShowError(string error);

		/// <summary>
		/// Executes the delegate in the UI thread.
		/// </summary>
		/// <param name="d">Delegate to execute</param>
		/// <param name="parameters">Parameters.</param>
		void ExecuteInUIThread(Delegate d, params object[] parameters);
		bool AskYesNo(string question, string title);
		string Ask(string question, string answer);
		bool HasChanges { get; set; }
		bool Save();
		string Title { get; set; }
		void Close(bool askToSave);
		bool SaveAs();
		event EventHandler Closed;
		event EventHandler TitleChanged;
	}
}