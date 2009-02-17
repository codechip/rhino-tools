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
using System.Collections.Generic;
using System.Text;
using System.Web;
using log4net;

namespace Rhino.Commons.HttpModules
{
    public class PagePerformanceModule : IHttpModule
    {
        static ILog logger = LogManager.GetLogger(typeof (PagePerformanceModule));
        static object PagePerformanceStartKey = new object();

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items[PagePerformanceStartKey] = DateTime.Now;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;
            DateTime startRequest = (DateTime) HttpContext.Current.Items[PagePerformanceStartKey];
            TimeSpan duration = DateTime.Now - startRequest;
            
            ThreadContext.Properties["page_duration"] = duration.ToString();
            ThreadContext.Properties["start_request"] = startRequest;
            ThreadContext.Properties["page_url"] = request.RawUrl;
            
            logger.InfoFormat("Processing {0} started at {1} took {2}", request.RawUrl, 
                              startRequest,duration);

            ResetLoggingProperties();
        }

        private static void ResetLoggingProperties()
        {
            ThreadContext.Properties["page_duration"] = null;
            ThreadContext.Properties["start_request"] = null;
            ThreadContext.Properties["page_url"] = null;
        }

        public void Dispose()
        {
        }
    }
}