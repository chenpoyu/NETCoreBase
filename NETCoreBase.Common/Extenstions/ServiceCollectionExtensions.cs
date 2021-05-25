using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Handlers;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Services;
using NETCoreBase.Common.Policies;
using System.IdentityModel.Tokens.Jwt;
using NETCoreBase.Database.Models;
using NETCoreBase.Database;
using Microsoft.AspNetCore.Mvc.Formatters;
using NETCoreBase.Common.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCoreModule(this IServiceCollection services, JwtTokenConfig options)
        {
            services.AddOauthJwt(options);
            services.AddPermission();
        }

        private static void AddOauthJwt(this IServiceCollection services, JwtTokenConfig options)
        {

            services.AddSingleton(options);

            // services.AddIdentity<User, Role>(options =>
            // {
            //     //options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.NameId;
            //     options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Sub;
            //     options.ClaimsIdentity.RoleClaimType = "role";
            //     options.SignIn.RequireConfirmedAccount = false;
            //     options.SignIn.RequireConfirmedEmail = false;
            //     options.Password.RequireDigit = true;
            //     options.Password.RequiredLength = 8;
            //     options.Password.RequireNonAlphanumeric = false;
            //     options.Password.RequireUppercase = true;
            //     options.Password.RequireLowercase = true;
            // })
            //     .AddEntityFrameworkStores<NETCoreBaseContext>();

            // ??
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove(JwtRegisteredClaimNames.Sub);
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("roles");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                {
                    x.Audience = options.Audience;

                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                        NameClaimType = JwtRegisteredClaimNames.Sub,
                        // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                        RoleClaimType = ClaimTypes.Role,
                        RequireExpirationTime = true,
                        
                        ValidateIssuer = true,
                        ValidIssuer = options.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Secret)),
                        ValidAudiences = new List<string> { options.Audience },
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMilliseconds(1)
                    };
                });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthorization(x =>
            {
                x.AddPolicy(JwtAuthPolicy.PolicyName, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.AddRequirements(new JwtAuthRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, JwtAuthHandler>();
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
        }

        private static void AddPermission(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddAuthorization(x =>
            {
                x.AddPolicy(PermissionPolicy.PolicyName,
                    policy => policy.AddRequirements(new PermissionRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IPermissionService, PermissionService>();
        }
        
        public static IMvcBuilder AddExcelOutputFormatter(this IMvcBuilder builder)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, ExcelOutputFormatterSetup>());

            return builder;

        }

        public class ExcelOutputFormatterSetup : IConfigureOptions<MvcOptions>
        {
            void IConfigureOptions<MvcOptions>.Configure(MvcOptions options)
            {
                options.OutputFormatters.Add(new ExcelOutputFormatter(new ExcelService()));
            }
        }
    }
}