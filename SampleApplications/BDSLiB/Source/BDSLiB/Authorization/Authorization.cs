namespace BDSLiB.Authorization
{
    using System;
    using System.IO;
    using System.Security.Principal;
    using BDSLiB.Properties;
    using BDSLiB.Authorization;
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
            //assume that operations starts with '/'
            string operationUnrooted = operation.Substring(1);
            AuthorizationRule authorizationRule =
                dslFactory.TryCreate<AuthorizationRule>(operation, principal, entity);
            if(authorizationRule == null)
            {
                return new AuthorizationResult(false, "No rule allow this operation");
            }
            authorizationRule.CheckAuthorization();
            return new AuthorizationResult(
                authorizationRule.Allowed,
                authorizationRule.Message
                );
        }

        #region Nested type: AuthorizationResult

        public class AuthorizationResult
        {
            public bool? Allowed;
            public string Message;

            public AuthorizationResult(bool? allowed, string message)
            {
                Allowed = allowed;
                Message = message;
            }
        }

        #endregion
    }
}