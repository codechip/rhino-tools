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
using System.Security.Principal;
using Rhino.Igloo;

namespace Rhino.Igloo.Tests
{
	public class FakeContext : IContext
	{
		private NameValueCollection inputs = new NameValueCollection();

		private string lastRedirectedUrl;
		private IDictionary<string, object> session = new Dictionary<string, object>();
	    private string lastUser;
	    private bool signOutCalled;
		private IPrincipal principal = new GenericPrincipal(new GenericIdentity("test"), null);
        private IList<UploadedFile> uploadedFiles = new List<UploadedFile>();
		private TimeSpan? refreshWaitTime;
		private string refreshUrl;
	    private bool hasEnded;
	    private string rawUrl = null;
	    public bool isIE7;

	    public void Redirect(string destination)
		{
			LastRedirectedUrl = destination;
		}

		public NameValueCollection Inputs
		{
			get { return inputs; }
		}


		public string GetInputVariable(string key)
		{
			return Inputs[key];
		}


	    public string[] GetMultiplyInputVariables(string key)
	    {
	        return inputs.GetValues(key);
	    }

	    public bool SignOutCalled
	    {
	        get { return signOutCalled; }
	        set { signOutCalled = value; }
	    }

	    public string LastUser
	    {
	        get { return lastUser; }
	        set { lastUser = value; }
	    }

	    public string LastRedirectedUrl
		{
			get { return lastRedirectedUrl; }
			set { lastRedirectedUrl = value; }
		}

		public IDictionary<string, object> Session
		{
			get { return session; }
			set { session = value; }
		}

		public object GetFromSession(string key)
		{
			if(Session.ContainsKey(key)==false)
				return null;
			return Session[key];
		}

		public void SetAtSession(string key, object value)
		{
			Session[key] = value;
		}

	    public void AuthenticateAndRedirect(string destination, string user)
	    {
	        lastRedirectedUrl = destination;
	        lastUser = user;
	    }

	    public void SignOut()
	    {
	        lastUser = null;
	        signOutCalled = true;
	    }


	    public IPrincipal CurrentUser
	    {
            get { return principal; }
	    }

	    public IList<UploadedFile> UploadedFiles
	    {
	        get { return uploadedFiles; }
	    }

	    public string RawUrl
	    {
	        get { return rawUrl; }
	    }

	    public string GetFullPath(string directory)
	    {
	        throw new NotImplementedException();
	    }

        public string GetHostName()
        {
            throw new NotImplementedException();
        }

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

		public string RefreshUrl
		{
			get { return refreshUrl; }
			set { refreshUrl = value; }
		}

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

	    public Uri Url()
	    {
	        return new Uri(lastRedirectedUrl);
	    }

	    public string UrlEncode(string s)
	    {
	        return s;
	    }

	    public string UrlDecode(string s)
	    {
	        return s;
	    }


	    public bool BrowserIsIE7()
	    {
	        return this.isIE7;
	    }

	    public string ResolveUrl(string url)
	    {
	        return url;
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
