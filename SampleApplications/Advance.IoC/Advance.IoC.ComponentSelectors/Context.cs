using System;

namespace Advance.IoC.ComponentSelectors
{
    public static class Context
    {
        [ThreadStatic]
        private static User currentUser;
        
        public static User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
        }   
    }
}