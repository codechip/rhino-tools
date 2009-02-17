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
using System.Security.Principal;
using Rhino.Igloo;

namespace Rhino.Igloo.Tests
{
    /// <summary>
    /// Fake context, useful for testing
    /// </summary>
    public class FakeContext : IContext
    {
        private readonly NameValueCollection inputs = new NameValueCollection();

        private string lastRedirectedUrl;
        private IDictionary<string, object> session = new Dictionary<string, object>();
        private string lastUser;
        private bool signOutCalled;
        private IPrincipal principal = new GenericPrincipal(new GenericIdentity("test"), null);
        private readonly IList<UploadedFile> uploadedFiles = new List<UploadedFile>();
        private TimeSpan? refreshWaitTime;
        private string refreshUrl;
        private bool hasEnded;
        private string rawUrl = null;
        private bool isIE7;
        private readonly IDictionary<string, string> context = new Dictionary<string, string>();

        /// <summary>
        /// Sets the principle
        /// </summary>
        /// <param name="value">the <see cref="IPrincipal"/> to use instead of <see cref="GenericPrincipal"/>.</param>
        public void SetPrincipal(IPrincipal value)
        {
            this.principal = value;
        }

        /// <summary>
        /// Sets the is IE 7 flag
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetIsIE7(bool value)
        {
            isIE7 = value;
        }

        /// <summary>
        /// Redirects the specified destination.
        /// </summary>
        /// <param name="destination"></param>
        public void Redirect(string destination)
        {
            LastRedirectedUrl = destination;
        }

        /// <summary>
        /// Redirects the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        public void Redirect(string destination, bool endResponse)
        {
            LastRedirectedUrl = destination;
        }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>The inputs.</value>
        public NameValueCollection Inputs
        {
            get { return inputs; }
        }


        /// <summary>
        /// Gets the variable from the user input
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetInputVariable(string key)
        {
            return Inputs[key];
        }


        /// <summary>
        /// Gets the input variables (multiplies of them).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string[] GetMultiplyInputVariables(string key)
        {
            return Inputs.GetValues(key);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sign out called].
        /// </summary>
        /// <value><c>true</c> if [sign out called]; otherwise, <c>false</c>.</value>
        public bool SignOutCalled
        {
            get { return signOutCalled; }
            set { signOutCalled = value; }
        }

        /// <summary>
        /// Gets or sets the last user.
        /// </summary>
        /// <value>The last user.</value>
        public string LastUser
        {
            get { return lastUser; }
            set { lastUser = value; }
        }

        /// <summary>
        /// Gets or sets the last redirected URL.
        /// </summary>
        /// <value>The last redirected URL.</value>
        public string LastRedirectedUrl
        {
            get { return lastRedirectedUrl; }
            set { lastRedirectedUrl = value; }
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public IDictionary<string, object> Session
        {
            get { return session; }
            set { session = value; }
        }

        /// <summary>
        /// Gets the variable from the session.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object GetFromSession(string key)
        {
            return Session.ContainsKey(key) ? Session[key] : null;
        }

        /// <summary>
        /// Sets the variable at the session level.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetAtSession(string key, object value)
        {
            Session[key] = value;
        }

        /// <summary>
        /// Authenticates the user and redirect to the destination
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        public void AuthenticateAndRedirect(string destination, string user)
        {
            lastRedirectedUrl = destination;
            lastUser = user;
        }

		/// <summary>
		/// Authenticates as the user name
		/// </summary>
		/// <param name="user">The user.</param>
    	public void AuthenticateAndRedirect(string user)
    	{
    		lastUser = user;
    	}

    	/// <summary>
        /// Authenticates the user and redirect to the destination
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        public void AuthenticateAndRedirect(string destination, string user, bool endResponse)
        {
            lastRedirectedUrl = destination;
            lastUser = user;
        }

		/// <summary>
		/// Authenticates as the user name
		/// </summary>
		/// <param name="user">The user.</param>
    	public void Authenticate(string user)
    	{
    		lastUser = user;
    	}

    	/// <summary>
        /// Signs the user out of the system
        /// </summary>
        public void SignOut()
        {
            lastUser = null;
            signOutCalled = true;
        }


        /// <summary>
        /// Gets the principal of the current user
        /// </summary>
        /// <value>The identity.</value>
        public IPrincipal CurrentUser
        {
            get { return principal; }
        }

        /// <summary>
        /// Gets the uploaded files.
        /// </summary>
        /// <value>The uploaded files.</value>
        public IList<UploadedFile> UploadedFiles
        {
            get { return uploadedFiles; }
        }

        /// <summary>
        /// The raw url that the user has sent
        /// </summary>
        /// <value></value>
        public string RawUrl
        {
            get { return rawUrl; }
			set { rawUrl = value; }
        }

        /// <summary>
        /// Gets the full path from a relative or ~/ one
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        public string GetFullPath(string directory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the host name
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensures that the directory exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public void EnsureDirectoryExists(string path)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Adds the refresh header to refresh the page after the waitTime is over.
        /// </summary>
        /// <param name="waitTime">The wait time.</param>
        public void AddRefreshHeaderAfter(TimeSpan waitTime)
        {
            this.RefreshWaitTime = waitTime;
        }

        /// <summary>
        /// Adds the refresh header to refresh the page after the waitTime is over.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="waitTime">The wait time.</param>
        public void AddRefreshHeaderAfter(string url, TimeSpan waitTime)
        {
            this.RefreshWaitTime = waitTime;
            this.RefreshUrl = url;
        }

        /// <summary>
        /// Gets or sets the refresh URL.
        /// </summary>
        /// <value>The refresh URL.</value>
        public string RefreshUrl
        {
            get { return refreshUrl; }
            set { refreshUrl = value; }
        }

        /// <summary>
        /// Gets or sets the refresh wait time.
        /// </summary>
        /// <value>The refresh wait time.</value>
        public TimeSpan? RefreshWaitTime
        {
            get { return refreshWaitTime; }
            set { refreshWaitTime = value; }
        }


        /// <summary>
        /// Maps the path (translate ~/ to the correct virtual path)
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            return path;
        }


        /// <summary>
        /// Html decode the string
        /// </summary>
        /// <param name="htmlEncodedString">The HTML encoded string.</param>
        /// <returns></returns>
        public string HtmlDecode(string htmlEncodedString)
        {
            return htmlEncodedString;
        }


        /// <summary>
        /// Html decode the string
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        /// <returns></returns>
        public string HtmlEncode(string htmlString)
        {
            return htmlString;
        }


        /// <summary>
        /// Ends the current request
        /// </summary>
        public void EndResponse()
        {
            HasEnded = true;
        }

        /// <summary>
        /// Return the Url of the request
        /// </summary>
        /// <returns></returns>
        public Uri Url()
        {
            return new Uri(lastRedirectedUrl);
        }

        /// <summary>
        /// Url encode the string
        /// </summary>
        /// <param name="s">The unencoded url</param>
        /// <returns>The encoded url</returns>
        public string UrlEncode(string s)
        {
            return s;
        }

        /// <summary>
        /// Url decode the string
        /// </summary>
        /// <param name="s">The encoded url</param>
        /// <returns>The unencoded url</returns>
        public string UrlDecode(string s)
        {
            return s;
        }


        /// <summary>
        /// Determines whether the browser is IE 7
        /// </summary>
        /// <returns></returns>
        public bool BrowserIsIE7()
        {
            return this.isIE7;
        }

        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string ResolveUrl(string url)
        {
            return url;
        }

        /// <summary>
        /// Sets the context varaible with the specified value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetContextVaraible(string name, object value)
        {
            context[name] = value.ToString();
        }

        /// <summary>
        /// Gets the logon user server variable
        /// </summary>
        /// <returns></returns>
        public string GetLogonUser()
        {
            return LastUser;
        }

        /// <summary>
        /// Check that the client ip address is in the given list.
        /// </summary>
        /// <param name="listOfIps"></param>
        /// <returns></returns>
        public bool ClientIpIsIn(ICollection listOfIps)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the session.
        /// </summary>
        public void ClearSession()
        {
            session.Clear();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has ended.
        /// </summary>
        /// <value><c>true</c> if this instance has ended; otherwise, <c>false</c>.</value>
        public bool HasEnded
        {
            get { return hasEnded; }
            set { hasEnded = value; }
        }
    }
}