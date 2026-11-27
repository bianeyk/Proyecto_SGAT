namespace SGAT.Web.Security
{
    public static class RateLimitPolicies
    {
        public const string Login = "LoginPolicy";

        public const string ForgotPassword =
            "ForgotPasswordPolicy";

        public const string ResetPassword =
            "ResetPasswordPolicy";

        public const string Search = "SearchPolicy";
    }
}