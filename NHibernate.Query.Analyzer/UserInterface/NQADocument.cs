#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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