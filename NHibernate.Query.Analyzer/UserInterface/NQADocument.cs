using System;
using System.ComponentModel;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using WeifenLuo.WinFormsUI;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	public class NQADocument : DockContent, IView
	{
		private readonly IView parentView;

		protected NQADocument(IView parentView)
		{
			this.parentView = parentView;

		}

		//FOR VS.NET DESIGNER ONLY!!!
		private NQADocument(){}

		private bool hasChanges = false;
		private string title;

		public bool HasChanges
		{
			get { return hasChanges; }
			set
			{
				hasChanges = value;
				SetTitle();
			}
		}

		public virtual bool Save()
		{
			return false;
		}

		public string Title
		{
			get { return title; }
			set
			{
				title = value;
				SetTitle();
			}
		}

		public void Close(bool askToSave)
		{
			if(askToSave==false)
				HasChanges = false;
			Close();
		}

		public virtual bool SaveAs()
		{
			return false;
		}

		public event EventHandler TitleChanged;

		private void SetTitle()
		{
			Text = title + (HasChanges ? " *" : "");
			if(TitleChanged!=null)
				TitleChanged(this,EventArgs.Empty);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel && HasChanges)
			{
				DialogResult response = MessageBox.Show("Do you want to save the changes?", Title, MessageBoxButtons.YesNoCancel);
				switch (response)
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
					case DialogResult.Yes:
						Save();
						break;
				}
				HasChanges = false;
			}
		}

		/// <summary>
		/// Starts the wait message by the UI. The view need to check every <c>checkInterval</c> 
		/// that the work was comleted (using <c>HasFinishedWork()</c> method).
		/// The work should finish in shouldWaitFor, but there is no gurantee about it.
		/// <c>EndWait</c> is called to end the wait.
		/// </summary>
		/// <param name="waitMessage">The Wait message.</param>
		/// <param name="checkInterval">Check interval.</param>
		/// <param name="shouldWaitFor">Should wait for.</param>
		public void StartWait(string waitMessage, int checkInterval, int shouldWaitFor)
		{
			parentView.StartWait(waitMessage, checkInterval, shouldWaitFor);
		}

		public void EndWait(string endMessage)
		{
			parentView.EndWait(endMessage);
		}

		public virtual void AddException(Exception ex)
		{
			parentView.AddException(ex);
		}

		public void ShowError(string error)
		{
			parentView.ShowError(error);
		}

		/// <summary>
		/// Executes the delegate in the UI thread.
		/// </summary>
		/// <param name="d">Delegate to execute</param>
		/// <param name="parameters">Parameters.</param>
		public void ExecuteInUIThread(Delegate d, params object[] parameters)
		{
			parentView.ExecuteInUIThread(d, parameters);
		}

		public bool AskYesNo(string question, string title)
		{
			return parentView.AskYesNo(question, title);
		}

		public string Ask(string question, string answer)
		{
			return parentView.Ask(question, answer);
		}
	}
}