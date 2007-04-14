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
            foreach (string formKey in context.Request.Form.AllKeys)
            {
                if (formKey != null && formKey.EndsWith(key))
                    return context.Request.Form[formKey];
            }
            return context.Request.QueryString[key];
        }


        /// <summary>
        /// Gets the input variables (multiplies of them).
        /// </summary>
        /// <param name="key">The key.</param>
        public string[] GetMultiplyInputVariables(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key", "The key cannnot be null");
            foreach (string formKey in context.Request.Form.AllKeys)
            {
                if (formKey != null && formKey.EndsWith(key))
                    return context.Request.Form.GetValues(formKey);
            }
            return context.Request.QueryString.GetValues(key); 
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
        /// Gets the identity.
        /// </summary>
        /// <value>The identity.</value>
        public IIdentity Identity
        {
            get { return context.User.Identity; }
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
    }
}