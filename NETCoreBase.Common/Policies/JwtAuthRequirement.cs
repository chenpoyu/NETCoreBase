using Microsoft.AspNetCore.Authorization;

namespace NETCoreBase.Common.Policies
{
    public class JwtAuthRequirement : IAuthorizationRequirement { }

    public static class JwtAuthPolicy
    {
        public const string PolicyName = "JwtAuthPolicy";
    }
}