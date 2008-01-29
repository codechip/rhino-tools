namespace Chapter5.Security
{
    using System;
    using System.Security.Principal;
    using MessageRouting;
    using Properties;
    using Rhino.DSL;

    public static class Authorization
    {
        private static readonly DslFactory dslFactory;

        static Authorization()
        {
            dslFactory = new DslFactory();
            dslFactory.Register<AuthorizationRule>(new AuthorizationDslEngine());
        }

        public static bool? IsAllowed(IPrincipal principal, string operation)
        {
            return ExecuteAuthorizationRules(principal, operation, null).Allowed;
        }

        public static bool? IsAllowed(IPrincipal principal, string operation, object entity)
        {
            return ExecuteAuthorizationRules(principal, operation, entity).Allowed;
        }


        public static string WhyAllowed(IPrincipal principal, string operation)
        {
            return ExecuteAuthorizationRules(principal, operation, null).Message;
        }

        public static string WhyAllowed(IPrincipal principal, string operation, object entity)
        {
            return ExecuteAuthorizationRules(principal, operation, entity).Message;
        }

        private static AuthorizationResult ExecuteAuthorizationRules(
            IPrincipal principal,
            string operation,
            object entity)
        {
            AuthorizationRule[] authorizationRules = dslFactory.CreateAll<AuthorizationRule>(
                Settings.Default.AuthorizationScriptsDirectory,
                principal, entity);
            foreach (AuthorizationRule rule in authorizationRules)
            {
                bool operationMatched = string.Equals(
                    rule.Operation, operation,
                    StringComparison.InvariantCultureIgnoreCase);

                if (operationMatched == false)
                    continue;

                rule.CheckAuthorization();
                if (rule.Allowed != null)
                {
                    return new AuthorizationResult(
                        rule.Allowed, 
                        rule.Message
                        );
                }
            }
            return new AuthorizationResult(
                null, 
                "No rule allowed this operation"
                );
        }

        #region Nested type: AuthorizationResult

        public class AuthorizationResult
        {
            public bool? Allowed;
            public string Message;

            public AuthorizationResult(bool? allowed, string mEssage)
            {
                Allowed = allowed;
                Message = mEssage;
            }
        }

        #endregion
    }
}