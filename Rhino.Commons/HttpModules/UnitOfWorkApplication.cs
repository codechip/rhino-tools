﻿#region license
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
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using Castle.Windsor;
using log4net;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
	public class UnitOfWorkApplication : HttpApplication, IContainerAccessor
	{
		private static IWindsorContainer windsorContainer;

		public UnitOfWorkApplication()
		{
			BeginRequest += new EventHandler(UnitOfWorkApplication_BeginRequest);
			EndRequest += new EventHandler(UnitOfWorkApplication_EndRequest);
		}

		public virtual void UnitOfWorkApplication_BeginRequest(object sender, EventArgs e)
		{
			if (IoC.IsInitialized == false)
				InitializeContainer(this);
			UnitOfWork.Start();
		}

		public virtual void UnitOfWorkApplication_EndRequest(object sender, EventArgs e)
		{
			IUnitOfWork unitOfWork = UnitOfWork.Current;
			if (unitOfWork != null)
				unitOfWork.Dispose();
		}

		public IWindsorContainer Container
		{
			get { return windsorContainer; }
		}

		public virtual void Application_Start(object sender, EventArgs e)
		{
			InitializeContainer(this);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void InitializeContainer(UnitOfWorkApplication self)
		{
			if (IoC.IsInitialized)
				return;
			self.CreateContainer();
		}


		public virtual void Application_End(object sender, EventArgs e)
		{
			if (windsorContainer != null) //can happen if this isn't the first app
			{
				IoC.Reset(windsorContainer);
				windsorContainer.Dispose();
			}
		}


		public override void Dispose()
		{
			BeginRequest -= new EventHandler(UnitOfWorkApplication_BeginRequest);
			EndRequest -= new EventHandler(UnitOfWorkApplication_EndRequest);
		}

		protected virtual void CreateContainer()
		{
			string windsorConfig = Settings.Default.WindsorConfig;
			if (!Path.IsPathRooted(windsorConfig))
			{
				//In ASP.Net apps, the current directory and the base path are NOT the same.
				windsorConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, windsorConfig);
			}
			windsorContainer = new RhinoContainer(windsorConfig);
			IoC.Initialize(windsorContainer);
		}
	}
}