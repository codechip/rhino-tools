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
using System.Web;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Rhino.Commons.Exceptions;
using Rhino.Commons.Properties;

namespace Rhino.Commons.HttpModules
{
	/// <summary>
	/// This verifies that the current request has performed less than the configurable 
	/// number of queries (MaxNumberOfQueriesPerRequest).
	/// Can be temporarily ignored using ?hack=true
	/// </summary>
	public class EnsureMaxNumberOfQueriesPerRequestModule : IHttpModule
	{
		static readonly object key = new object();
		private int maxNumberOfQueriesPerRequest = Settings.Default.MaxNumberOfQueriesPerRequest;

		public void Init(HttpApplication context)
		{
			// we need to make the check here because an HttpModule can be created more then
			// once in an application lifetime, so we should be careful not to add the counter
			// twice.
			Logger logger = (Logger)LogManager.GetLogger("NHibernate.SQL").Logger;
			lock (logger)
			{
				if (HasCountingAppender(logger) == false)
					logger.AddAppender(new CountToContextItemsAppender());
			}
			context.EndRequest += new EventHandler(context_EndRequest);
		}

		private static bool HasCountingAppender(Logger logger)
		{
			foreach (object appender in logger.Appenders)
			{
				if (appender is CountToContextItemsAppender)
					return true;
			}
			return false;
		}

		void context_EndRequest(object sender, EventArgs e)
		{
			// this is a using ?hack=<something>, we allow this, probably the developer is developing the page
			if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["hack"]) == false)
				return;
			object maybeVal = HttpContext.Current.Items[key];
			int count = 0;
			if (maybeVal != null)
				count = (int)maybeVal;
			if (count > maxNumberOfQueriesPerRequest)
			{
				throw new PerformancePolicyViolationException(
					string.Format(
						@"Only {0} queries are allowed per web request, but {1} queries were perfromed during this web request. 
Optimize the data access code. You can continue working by adding the query string parameter hack=true.",
						maxNumberOfQueriesPerRequest, count));
			}
		}

		public void Dispose()
		{
		}

		public class CountToContextItemsAppender : IAppender
		{
			private string name;

			public void Close()
			{

			}

			public void DoAppend(LoggingEvent loggingEvent)
			{
				object maybeVal = HttpContext.Current.Items[key];
				int current = 0;
				if (maybeVal != null)
					current = (int)maybeVal;
				HttpContext.Current.Items[key] = current + 1;
			}

			public string Name
			{
				get { return name; }
				set { name = value; }
			}
		}
	}
}
