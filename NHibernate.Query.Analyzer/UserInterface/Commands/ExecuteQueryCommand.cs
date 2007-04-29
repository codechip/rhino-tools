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
using System.Collections;
using System.Data;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Commands
{
	public class ExecuteQueryCommand : ICommand
	{
		private IQueryView view;
		private Project prj;
		public string hqlQueryText;

		public ExecuteQueryCommand(IQueryView view, Project prj, string hqlQueryText, TypedParameter[] typedParameters)
		{
			this.view = view;
			this.prj = prj;
			this.hqlQueryText = hqlQueryText;
			this.typedParameters = typedParameters;
		}

		public TypedParameter[] typedParameters;

		#region ICommand Members

		public void Execute()
		{
			try
			{
				IList list = prj.RunHql(this.hqlQueryText, this.typedParameters);
				DataSet ds = prj.RunHqlAsRawSql(this.hqlQueryText, this.typedParameters);
				view.ExecuteInUIThread(new JobFinishedSuccessfully(QueryFinishedSuccessfully), list, ds);
			}
			catch (Exception ex)
			{
				view.ExecuteInUIThread(new JobFailed(QueryFailed), ex);
			}

		}

		public string Info
		{
			get { return "Query: " + hqlQueryText; }
		}

		public string Name
		{
			get { return GetType().Name; }
		}

		#endregion

		#region Implementation

		private delegate void JobFailed(Exception ex);

		private delegate void JobFinishedSuccessfully(IList list, DataSet ds);

		private void QueryFailed(Exception ex)
		{
			view.AddException(ex);
			view.EndWait("An exception occured executing query");
		}

		private void QueryFinishedSuccessfully(IList list, DataSet ds)
		{
			view.DisplayObjectGraph(list);
			view.DisplayDataSet(ds);
			view.ShowObjectGraph();
			view.EndWait("Finished executing query");
		}

		#endregion
	}
}