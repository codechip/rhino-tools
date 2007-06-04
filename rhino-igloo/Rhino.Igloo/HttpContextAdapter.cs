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
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using Castle.Core;

namespace Rhino.Igloo
{
    /// <summary>
    /// Adapter from HttpContext to IContext.
    /// Used because we don't want to get a dependency on the real HttpAdapter, since
    /// that would prevent successful testsing.
    /// </summary>
    [Transient]
    public class HttpContextAdapter : IContext
    {
        private HttpContext context;
        private IList<UploadedFile> uploadedFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextAdapter"/> class.
        /// </summary>
        /// <param myName="context">The context.</param>
        public HttpContextAdapter(HttpContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Redirects to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void Redirect(string destination)
        {
            destination = ((Page) context.Handler).ResolveUrl(destination);
            // NOTE: This must not throw exception (which it would do if endRequest is true
            // because it is called from transactional methods.
            context.Response.Redirect(destination, false);
        }

        /// <summary>
        /// Gets the input variable.
        /// </summary>
        /// <param name="key">The key.</param>
        public string GetInputVariable(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "The key cannnot be null");
			string aspNetFormKey = "$" + key;
			foreach (string formKey in context.Request.Form.AllKeys)
            {
				if (formKey != null && (formKey.EndsWith(aspNetFormKey) || key == formKey))
					return context.Server.HtmlEncode(context.Request.Form[formKey]);
            }
        	string result = context.Request.QueryString[key];
			if (string.IsNullOrEmpty(result) == false)
				return context.Server.HtmlEncode(result);
        	return result;
        }


        /// <summary>
        /// Gets the input variables (multiplies of them).
        /// </summary>
        /// <param name="key">The key.</param>
        public string[] GetMultiplyInputVariables(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "The key cannnot be null");
			string aspNetFormKey = "$" + key;
			foreach (string formKey in context.Request.Form.AllKeys)
            {
				if (formKey != null && (formKey.EndsWith(aspNetFormKey) || key==formKey) )
				{
					string[] results = context.Request.Form.GetValues(formKey);
					return HtmlEncode(results);
				}
            }
        	return HtmlEncode(context.Request.QueryString.GetValues(key));
        }

		private string[] HtmlEncode(string[] results)
    	{
            if (results==null)
                return new string[0];
    		for (int i = 0; i < results.Length; i++)
    		{
				if(string.IsNullOrEmpty(results[i])==false)
				{
					results[i] = context.Server.HtmlEncode(results[i]);
				}
    		}
    		return results;
    	}

    	/// <summary>
        /// Gets from the session.
        /// </summary>
        /// <param name="key">The key.</param>
        public object GetFromSession(string key)
        {
            if(context.Session==null)
                return null;
            return context.Session[key];
        }

        /// <summary>
        /// Sets the value at the session level.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetAtSession(string key, object value)
        {
            if (context.Session == null)
                throw new InvalidOperationException("Not in a session");
            context.Session[key] = value;
        }

        /// <summary>
        /// Authenticates the user and redirect to the desination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        public void AuthenticateAndRedirect(string destination, string user)
        {
            FormsAuthenticationTicket authTicket =
                new FormsAuthenticationTicket(user, false, 60);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            HttpCookie authCookie =
                new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            context.Response.Cookies.Add(authCookie);
            context.Response.Redirect(destination);
        }

        /// <summary>
        /// Signs the user out of the system
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }


        /// <summary>
        /// Gets the principal of the current user.
        /// </summary>
        /// <value>The principal.</value>
        public IPrincipal CurrentUser
        {
            get { return context.User; }
        }

        /// <summary>
        /// Gets the uploaded files.
        /// </summary>
        /// <value>The uploaded files.</value>
        public IList<UploadedFile> UploadedFiles
        {
            get
            {
                if (uploadedFiles == null)
                {
                    uploadedFiles = new List<UploadedFile>();
                    foreach (string key in context.Request.Files)
                    {
                        HttpPostedFile file = context.Request.Files[key];
                        if (string.IsNullOrEmpty(file.FileName))
                            continue;
                        uploadedFiles.Add(new UploadedFile(file.FileName, file.InputStream));
                    }
                }
                return uploadedFiles;
            }
        }

        /// <summary>
        /// Gets the full path from a relative or ~/ one
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        public string GetFullPath(string directory)
        {
            return context.Server.MapPath(directory);
        }

        /// <summary>
        /// Ensures that the directory exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }


    	/// <summary>
    	/// Adds the refresh header to refresh the page after the waitTime is over.
    	/// </summary>
    	/// <param name="waitTime">The wait time.</param>
    	public void AddRefreshHeaderAfter(TimeSpan waitTime)
    	{
    		context.Response.AddHeader("Refresh", waitTime.Seconds.ToString());
    	}

		/// <summary>
		/// Adds the refresh header to refresh the page after the waitTime is over.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="waitTime">The wait time.</param>
		public void AddRefreshHeaderAfter(string url, TimeSpan waitTime)
		{
			url = ((Page)context.Handler).ResolveUrl(url);
			context.Response.AddHeader("Refresh", string.Format("{0}; URL={1}", waitTime.Seconds, url));
		}


        /// <summary>
        /// Maps the path (translate ~/ to the correct virtual path)
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            return context.Response.ApplyAppPathModifier(path);
        }


        /// <summary>
        /// Html decode the string
        /// </summary>
        /// <param name="htmlEncodedString">The HTML encoded string.</param>
        /// <returns></returns>
        public string HtmlDecode(string htmlEncodedString)
        {
            return context.Server.HtmlDecode(htmlEncodedString);
        }

        /// <summary>
        /// Html decode the string
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        /// <returns></returns>
        public string HtmlEncode(string htmlString)
        {
            return context.Server.HtmlEncode(htmlString);
        }
    }
}
