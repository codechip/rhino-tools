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

		/// <summary>
		/// Gets the identity of the current user
		/// </summary>
		/// <value>The identity.</value>
		public IIdentity Identity
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
	}
}