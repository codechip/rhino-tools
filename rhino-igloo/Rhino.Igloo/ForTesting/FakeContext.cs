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
		private IIdentity identity = new GenericIdentity("test");
        private IList<UploadedFile> uploadedFiles = new List<UploadedFile>();

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

		public IIdentity Identity
		{
			get { return identity; }
		}

	    public IList<UploadedFile> UploadedFiles
	    {
	        get { return uploadedFiles; }
	    }

	    public string GetFullPath(string directory)
	    {
	        throw new NotImplementedException();
	    }

	    public void EnsureDirectoryExists(string path)
	    {
	        throw new NotImplementedException();
	    }
	}
}
