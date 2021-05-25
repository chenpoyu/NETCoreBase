using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NETCoreBase.Common;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Policies;

namespace NETCoreBase.Common.Handlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class PermissionAttribute : Attribute
    {
        public string Permission { get; private set; }

        public PermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
    
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var authorizationFilterContext = context.Resource as AuthorizationFilterContext;
            var descriptor = authorizationFilterContext.ActionDescriptor;
            var permissionAttribute = descriptor.EndpointMetadata.OfType<PermissionAttribute>().ToList();

            if (permissionAttribute == null || permissionAttribute.Count == 0)
            {
                // 若未設定特殊權限，則皆通過
                context.Succeed(requirement);
                return;
            }

            var list = new List<string>();
            permissionAttribute.ForEach((p) => list.Add($"{descriptor.RouteValues["controller"]}/{p.Permission}"));

            if (await _permissionService.HasPermission(context.User, list))
                context.Succeed(requirement);

            return;
        }
    }
}