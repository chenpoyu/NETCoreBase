using Microsoft.AspNetCore.Authorization;
using System;

namespace NETCoreBase.Common.Policies
{
    public class PermissionRequirement : IAuthorizationRequirement
    { }

    public static class PermissionPolicy
    {
        public const string PolicyName = "Permission";

        public const string Insert = "insert";

        public const string Query = "query";

        public const string Update = "update";

        public const string Delete = "delete";

        public const string Print = "print";

        public const string Export = "export";

        public const string Upload = "upload";

        public const string Action = "action";
    }
}