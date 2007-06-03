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
using System.Security.Principal;

namespace Rhino.Igloo
{
	public class ThrowingContextAdapter : IContext
	{
		/// <summary>
		/// Redirects the specified destination.
		/// </summary>
		/// <param myName="destination">The destination.</param>
		public void Redirect(string destination)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Gets the variable from the user input
		/// </summary>
		/// <param name="key">The key.</param>
		public string GetInputVariable(string key)
		{
			return null;
		}

		/// <summary>
		/// Gets the input variables (multiplies of them).
		/// </summary>
		/// <param name="key">The key.</param>
		public string[] GetMultiplyInputVariables(string key)
		{
			return new string[0];
		}

		/// <summary>
		/// Gets the variable from the session.
		/// </summary>
		/// <param name="key">The key.</param>
		public object GetFromSession(string key)
		{
			return null;
		}

		/// <summary>
		/// Sets the variable at the session level.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void SetAtSession(string key, object value)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Authenticates the user and redirect to the destination
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="user">The user.</param>
		public void AuthenticateAndRedirect(string destination, string user)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Signs the user out of the system
		/// </summary>
		public void SignOut()
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}


	    public IPrincipal CurrentUser
	    {
            get { throw new NotSupportedException("You are not in a web context, you cannot call the context"); } 
	    }

	    /// <summary>
		/// Gets the uploaded files.
		/// </summary>
		/// <value>The uploaded files.</value>
		public IList<UploadedFile> UploadedFiles
		{
			get { throw new NotSupportedException("You are not in a web context, you cannot call the context"); }
		}

		/// <summary>
		/// Gets the full path from a relative or ~/ one
		/// </summary>
		/// <param name="directory">The directory.</param>
		/// <returns></returns>
		public string GetFullPath(string directory)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Ensures that the directory exists.
		/// </summary>
		/// <param name="path">The path.</param>
		public void EnsureDirectoryExists(string path)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Adds the refresh header to refresh the page after the waitTime is over.
		/// </summary>
		/// <param name="waitTime">The wait time.</param>
		public void AddRefreshHeaderAfter(TimeSpan waitTime)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}

		/// <summary>
		/// Adds the refresh header to refresh the page after the waitTime is over.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="waitTime">The wait time.</param>
		public void AddRefreshHeaderAfter(string url, TimeSpan waitTime)
		{
			throw new NotSupportedException("You are not in a web context, you cannot call the context");
		}


	    /// <summary>
	    /// Maps the path (translate ~/ to the correct virtual path)
	    /// </summary>
	    /// <param name="path">The path.</param>
	    /// <returns></returns>
	    public string MapPath(string path)
	    {
            throw new NotSupportedException("You are not in a web context, you cannot call the context");
	    }
	}
}
