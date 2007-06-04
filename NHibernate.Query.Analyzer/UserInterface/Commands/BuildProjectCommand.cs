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
using System.Text;
using Ayende.NHibernateQueryAnalyzer.Core.Model;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Commands
{
	public class BuildProjectCommand : ICommand
	{
		private readonly IProjectsRepository repository;
		private readonly Project prj;
		public IProjectView view;

		public BuildProjectCommand(IProjectView view, Project prj, IProjectsRepository repository)
		{
			this.repository = repository;
			this.view = view;
			this.prj = prj;

		}

		public void Execute()
		{
			try
			{
				prj.BuildProject();
				view.ExecuteInUIThread(new JobDoneSuccess(BuildingProjectFinishedSuccessfully), prj);
			}
			catch (Exception ex)
			{
				view.ExecuteInUIThread(new JobFailed(BuildingProjectFailed), ex);
			}
		}

		public string Info
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Project: ").Append(prj.Name).Append("\r\n");
				sb.Append("Files:\r\n");
				foreach (string file in prj.Files)
				{
					sb.Append("\t").Append(file).Append("\r\n");
				}
				return sb.ToString();
			}
		}

		public string Name
		{
			get { return GetType().Name; }
		}

		#region Implementation Details

		private delegate void JobDoneSuccess(Project prj);

		private delegate void JobFailed(Exception ex);

		private void BuildingProjectFinishedSuccessfully(Project prj)
		{
			repository.SaveProject(prj);//to save the isBuildSuccessfully flag.
			view.DisplayProjectState(false, true);
			view.EndWait("Project was built successfully");
		}

		private void BuildingProjectFailed(Exception ex)
		{
			repository.SaveProject(prj);//to save the isBuildSuccessfully flag.
			view.EndWait(ex.Message);
			view.ShowError(ex.ToString());
			view.DisplayProjectState(true, false);
		}

		#endregion
	}
}