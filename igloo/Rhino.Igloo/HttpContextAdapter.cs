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
            destination = context.Response.ApplyAppPathModifier(destination);
            // NOTE: This must not throw exception (which it would do if endRequest is true
            // because it is called from transactional methods.
            Redirect(destination, true);
        }

        /// <summary>
        /// Redirects to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        public void Redirect(string destination, bool endResponse)
        {
            destination = context.Response.ApplyAppPathModifier(destination);
            // NOTE: This must not throw exception (which it would do if endRequest is true
            // because it is called from transactional methods.
            context.Response.Redirect(destination, endResponse);
        }

        /// <summary>
        /// Gets the input variable.
        /// </summary>
        /// <param name="key">The key.</param>
        public string GetInputVariable(string key)
        {
            if (key == null) throw new ArgumentNullException("key", "The key cannnot be null");

			string aspNetFormKey = "$" + key;
			foreach (string formKey in context.Request.Form.AllKeys)
            {
				if (formKey != null && (formKey.EndsWith(aspNetFormKey) || key == formKey))
					return context.Server.HtmlEncode(context.Request.Form[formKey]);
            }
        	string result = context.Request.QueryString[key];
			if (!string.IsNullOrEmpty(result))
				return context.Server.HtmlEncode(result);
        	return result;
        }


        /// <summary>
        /// Gets the input variables (multiplies of them).
        /// </summary>
        /// <param name="key">The key.</param>
        public string[] GetMultiplyInputVariables(string key)
        {
            if (key == null) throw new ArgumentNullException("key", "The key cannnot be null");

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
            if (results == null) return new string[0];

    		for (int i = 0; i < results.Length; i++)
    		{
				if(!string.IsNullOrEmpty(results[i]))
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
            return context.Session != null ? context.Session[key] : null;
        }

        /// <summary>
        /// Sets the value at the session level.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetAtSession(string key, object value)
        {
            if (context.Session == null) throw new InvalidOperationException("Not in a session");
            context.Session[key] = value;
        }

        /// <summary>
        /// Authenticates the user and redirect to the desination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        public void AuthenticateAndRedirect(string destination, string user)
        {
            AuthenticateAndRedirect(destination, user, true);
        }

        /// <summary>
        /// Authenticates the user and redirect to the desination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        public void AuthenticateAndRedirect(string destination, string user, bool endResponse)
        {
        	Authenticate(user);
        	context.Response.Redirect(destination, endResponse);
        }

    	/// <summary>
		/// Authenticates the specified user.
		/// </summary>
		/// <param name="user">The user.</param>
    	public void Authenticate(string user)
    	{
    		FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(user, false, 60);
    		HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
    		context.Response.Cookies.Add(authCookie);
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
                if (uploadedFiles != null) return uploadedFiles;

                uploadedFiles = new List<UploadedFile>();
                foreach (string key in context.Request.Files)
                {
                    HttpPostedFile file = context.Request.Files[key];
                    if (string.IsNullOrEmpty(file.FileName)) continue;
                    uploadedFiles.Add(new UploadedFile(file.FileName, file.InputStream));
                }
                return uploadedFiles;
            }
        }


        /// <summary>
        /// Gets the raw URL.
        /// </summary>
        /// <value>The raw URL.</value>
        public string RawUrl
        {
            get { return context.Request.RawUrl; }
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
        /// Gets the host name
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            return context.Request.Url.Host;
        }

        /// <summary>
        /// Ensures that the directory exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
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
			url = context.Response.ApplyAppPathModifier(url);
			context.Response.AddHeader("Refresh", string.Format("{0}; URL={1}", waitTime.Seconds, url));
		}


        /// <summary>
        /// Maps the path (translate ~/ to the correct virtual path)
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            return context.Server.MapPath(path);
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


        /// <summary>
        /// Ends the current request
        /// </summary>
        public void EndResponse()
        {
            context.Response.End();
        }

        /// <summary>
        /// Return the Url of the request.
        /// </summary>
        /// <returns></returns>
        public Uri Url()
        {
            return context.Request.Url;
        }

        /// <summary>
        /// Url encode the string
        /// </summary>
        /// <param name="s">The unencoded url</param>
        /// <returns>The encoded url</returns>
        public string UrlEncode(string s)
        {
            return context.Server.UrlEncode(s);
        }

        /// <summary>
        /// Url decode the string
        /// </summary>
        /// <param name="s">The encoded url</param>
        /// <returns>The unencoded url</returns>
        public string UrlDecode(string s)
        {
            return context.Server.UrlDecode(s);
        }

        /// <summary>
        /// Determines whether the browser is IE 7
        /// </summary>
        /// <returns></returns>
        public bool BrowserIsIE7()
        {
            HttpBrowserCapabilities browser = context.Request.Browser;
            return browser.Browser == "IE" && browser.MajorVersion == 7;
        }


        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string ResolveUrl(string url)
        {
            return context.Response.ApplyAppPathModifier(url);
        }


        /// <summary>
        /// Check that the client ip address is in the given list.
        /// </summary>
        /// <param name="listOfIps"></param>
        /// <returns></returns>
        public bool ClientIpIsIn(ICollection listOfIps)
        {
            string clientIp = context.Request.UserHostAddress;
            foreach (string ip in listOfIps)
            {
                string ipWithDot = ip;
                if (!ipWithDot.EndsWith("."))
                {
                    string[] split = ipWithDot.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 4) ipWithDot = ipWithDot + ".";
                }
                if (clientIp.StartsWith(ipWithDot)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the logon user server variable
        /// </summary>
        /// <returns></returns>
        public string GetLogonUser()
        {
            return context.Request.ServerVariables["LOGON_USER"];
        }

        /// <summary>
        /// Sets the context varaible with the specified value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetContextVaraible(string name, object value)
        {
            context.Items[name] = value;    
        }

        /// <summary>
        /// Clears the session.
        /// </summary>
        public void ClearSession()
        {
            context.Session.Clear();
        }
    }
}
