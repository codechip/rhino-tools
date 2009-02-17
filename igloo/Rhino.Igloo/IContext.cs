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

namespace Rhino.Igloo
{
	/// <summary>
	/// Abstract the HttpContext into an interface that can be mocked.
	/// </summary>
	public interface IContext
	{
		/// <summary>
		/// Redirects the specified destination.
		/// </summary>
		/// <param myName="destination">The destination.</param>
		void Redirect(string destination);

        /// <summary>
        /// Redirects the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        void Redirect(string destination, bool endResponse);

		/// <summary>
		/// Gets the variable from the user input
		/// </summary>
		/// <param name="key">The key.</param>
		string GetInputVariable(string key);

	    /// <summary>
	    /// Gets the input variables (multiplies of them).
	    /// </summary>
	    /// <param name="key">The key.</param>
	    string[] GetMultiplyInputVariables(string key);

		/// <summary>
		/// Gets the variable from the session.
		/// </summary>
		/// <param name="key">The key.</param>
		object GetFromSession(string key);

		/// <summary>
		/// Sets the variable at the session level.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void SetAtSession(string key, object value);

		/// <summary>
		/// Authenticates the user and redirect to the destination
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="user">The user.</param>
	    void AuthenticateAndRedirect(string destination,string user);

        /// <summary>
        /// Authenticates the user and redirect to the destination
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="user">The user.</param>
        /// <param name="endResponse">if set to <c>true</c> [end response].</param>
        void AuthenticateAndRedirect(string destination, string user, bool endResponse);

		/// <summary>
		/// Authenticates as the user name
		/// </summary>
		/// <param name="user">The user.</param>
	    void Authenticate(string user);

		/// <summary>
		/// Signs the user out of the system
		/// </summary>
	    void SignOut();

		/// <summary>
		/// Gets the principal of the current user
		/// </summary>
		/// <value>The identity.</value>
		IPrincipal CurrentUser { get; }

        /// <summary>
        /// Gets the uploaded files.
        /// </summary>
        /// <value>The uploaded files.</value>
        IList<UploadedFile> UploadedFiles { get; }

        /// <summary>
        /// The raw url that the user has sent
        /// </summary>
	    string RawUrl { get; }

	    /// <summary>
        /// Gets the full path from a relative or ~/ one
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
	    string GetFullPath(string directory);

        /// <summary>
        /// Gets the host name
        /// </summary>
        /// <returns></returns>
	    string GetHostName();

        /// <summary>
        /// Ensures that the directory exists.
        /// </summary>
        /// <param name="path">The path.</param>
	    void EnsureDirectoryExists(string path);

		/// <summary>
		/// Adds the refresh header to refresh the page after the waitTime is over.
		/// </summary>
		/// <param name="waitTime">The wait time.</param>
		void AddRefreshHeaderAfter(TimeSpan waitTime);

		/// <summary>
		/// Adds the refresh header to refresh the page after the waitTime is over.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="waitTime">The wait time.</param>
		void AddRefreshHeaderAfter(string url, TimeSpan waitTime);

        /// <summary>
        /// Maps the path (translate ~/ to the correct virtual path)
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
	    string MapPath(string path);

        /// <summary>
        /// Html decode the string
        /// </summary>
        /// <param name="htmlEncodedString">The HTML encoded string.</param>
        /// <returns></returns>
	    string HtmlDecode(string htmlEncodedString);

	    /// <summary>
	    /// Html decode the string
	    /// </summary>
	    /// <param name="htmlString">The HTML string.</param>
	    /// <returns></returns>
	    string HtmlEncode(string htmlString);

        /// <summary>
        /// Ends the current request
        /// </summary>
	    void EndResponse();


	    /// <summary>
	    /// Return the Url of the request
	    /// </summary>
	    /// <returns></returns>
	    Uri Url();

	    /// <summary>
	    /// Url encode the string
	    /// </summary>
	    /// <param name="s">The unencoded url</param>
	    /// <returns>The encoded url</returns>
	    string UrlEncode(string s);

	    /// <summary>
	    /// Url decode the string
	    /// </summary>
	    /// <param name="s">The encoded url</param>
	    /// <returns>The unencoded url</returns>
	    string UrlDecode(string s);

        /// <summary>
        /// Determines whether the browser is IE 7
        /// </summary>
	    bool BrowserIsIE7();

        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
	    string ResolveUrl(string url);

        /// <summary>
        /// Clears the session.
        /// </summary>
	    void ClearSession();

        /// <summary>
        /// Check that the client ip address is in the given list.
        /// </summary>
	    bool ClientIpIsIn(ICollection listOfIps);

        /// <summary>
        /// Gets the logon user server variable
        /// </summary>
	    string GetLogonUser();

        /// <summary>
        /// Sets the context varaible with the specified value
        /// </summary>
	    void SetContextVaraible(string name, object value);
	}
}
