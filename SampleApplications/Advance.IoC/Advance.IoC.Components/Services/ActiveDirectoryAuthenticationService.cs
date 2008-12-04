using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Advance.IoC.Components.Services
{
    public class ActiveDirectoryAuthenticationService : IAuthenticationService
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );
        
        public bool IsValidLogin(string user, string pass)
        {
            //IntPtr phToken;
            //return LogonUser(user, null, pass, 2, 0, out phToken);

            return user == "test" && pass == "fish";
        }

        public void SetPassword(string user, string pass)
        {
            Console.WriteLine("Ignoring new password for: " + user);
        }
    }
}