using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using NETCoreBase.Common;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Policies;

namespace NETCoreBase.Common.Handlers
{
    public class JwtAuthHandler : AuthorizationHandler<JwtAuthRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtAuthManager _jwtAuthManager;
        
        public JwtAuthHandler(IHttpContextAccessor httpContextAccessor, IJwtAuthManager jwtAuthManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtAuthManager = jwtAuthManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtAuthRequirement requirement)
        {
            var authorizationFilterContext = context.Resource as AuthorizationFilterContext;
            var token = _httpContextAccessor.HttpContext?.GetTokenAsync("access_token")?.Result;

            var (claimsPrincipal, jwtSecurityToken) = _jwtAuthManager.DecodeJwtToken(token);

            // TODO
            // if (jwtSecurityToken.)
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

    }
}