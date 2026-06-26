using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NETCoreBase.Common.Filter.Swagger
{
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymous = context.MethodInfo
                .GetCustomAttributes<AllowAnonymousAttribute>(true);

            if (allowAnonymous.Any())
                return;

            var actionAthorize = context.MethodInfo
                .GetCustomAttributes<AuthorizeAttribute>(true);

            var controllerAthorize = context.MethodInfo.DeclaringType
                .GetCustomAttributes<AuthorizeAttribute>(true);

            if (actionAthorize.Any() || controllerAthorize.Any())
            {
                if (operation.Responses == null)
                    operation.Responses = new OpenApiResponses();

                if (!operation.Responses.ContainsKey("401"))
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                if (!operation.Responses.ContainsKey("403"))
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                if (operation.Security == null)
                    operation.Security = new List<OpenApiSecurityRequirement>();

                // In Microsoft.OpenApi v2, use OpenApiSecuritySchemeReference instead of
                // OpenApiSecurityScheme with an inline Reference property.
                var scheme = new OpenApiSecuritySchemeReference("Bearer", null, null);
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }
    }
}
