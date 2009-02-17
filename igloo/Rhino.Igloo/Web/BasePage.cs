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
using System.IO.Compression;
using System.Web.UI;
using Rhino.Commons.HttpModules;

namespace Rhino.Igloo
{
    /// <summary>
    /// Base class for pages
    /// </summary>
    [View]
    public class BasePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePage"/> class.
        /// </summary>
        public BasePage()
        {
            Init += new EventHandler(CommonWebUI.WebUI_InjectComponent);
        }

        /// <summary>
        /// Adds the service reference.
        /// </summary>
        /// <param myName="path">The path.</param>
        public void AddServiceReference(string path)
        {
            ServiceReference reference = new ServiceReference(path);
            ((BaseMaster) Master).ScriptManager.Services.Add(reference);
        }

        /// <summary>
        /// Use gzip / default compression is the client allows this.
        /// </summary>
        public void EnableCompression()
        {
            string acceptEnconding = Request.Headers["Accept-encoding"];
            if (acceptEnconding != null &&
                acceptEnconding.Contains("gzip"))
            {
                Response.Filter = new GZipStream(Response.Filter,
                                                 CompressionMode.Compress, true);
                Response.AppendHeader("Content-encoding", "gzip");
            }
            else if (acceptEnconding != null &&
                     acceptEnconding.Contains("deflate"))
            {
                Response.Filter = new DeflateStream(Response.Filter,
                                                    CompressionMode.Compress, true);
                Response.AppendHeader("Content-encoding", "deflate");
            }
        }
    }
}