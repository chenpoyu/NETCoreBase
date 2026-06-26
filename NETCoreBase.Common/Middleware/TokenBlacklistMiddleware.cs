using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NETCoreBase.Common.Interfaces;

namespace NETCoreBase.Common.Middleware
{
    /// <summary>
    /// Rejects requests whose Bearer token appears in the JWT blacklist.
    /// Must be placed after UseAuthentication() and before UseAuthorization().
    /// </summary>
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtAuthManager _jwtAuthManager;

        public TokenBlacklistMiddleware(RequestDelegate next, IJwtAuthManager jwtAuthManager)
        {
            _next = next;
            _jwtAuthManager = jwtAuthManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (_jwtAuthManager.IsBlacklisted(token))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new { status = 401, title = "Token 已失效" }));
                    return;
                }
            }

            await _next(context);
        }
    }
}
