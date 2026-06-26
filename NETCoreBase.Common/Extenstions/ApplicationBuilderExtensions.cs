using NETCoreBase.Common.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCoreModule(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            // Feature 3: Reject blacklisted tokens after authentication validates the signature
            app.UseMiddleware<TokenBlacklistMiddleware>();
            app.UseAuthorization();
            return app;
        }
    }
}
