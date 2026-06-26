using System;
using System.IO;
using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using NETCoreBase.API.HealthChecks;
using NETCoreBase.Common;
using NETCoreBase.Common.Filter;
using NETCoreBase.Common.Filter.Swagger;
using NETCoreBase.Common.Middleware;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Policies;
using NETCoreBase.Core;
using NETCoreBase.Core.Behaviors;
using Prometheus;
using Serilog;
using MediatR;

namespace NETCoreBase.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new EFModule(Configuration.GetConnectionString("DefaultConnection")));
            builder.RegisterModule(new AutoMapperModule());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("BaseCorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials()
                );
            });

            var coreModuleOptions = (JwtTokenConfig)Configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfig>();
            services.AddCoreModule(coreModuleOptions);
            services.AddScoped<HttpResponseExceptionFilter>();

            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database");

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("account", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0,
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        type = "about:blank",
                        title = "請求過於頻繁，請稍後再試",
                        status = 429,
                    }, cancellationToken);
                };
            });

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<MediatorModule>();
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc();

            var bearerPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            services.AddControllers(c =>
            {
                c.RespectBrowserAcceptHeader = true;
                c.Filters.Add(new AuthorizeFilter(bearerPolicy));
                c.Filters.Add(new AuthorizeFilter(PermissionPolicy.PolicyName));
                c.Filters.Add(typeof(HttpResponseExceptionFilter));
            }).AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<MediatorModule>();
            })
            .AddExcelOutputFormatter();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NET Core Base",
                    Version = "v1",
                    Description = "使用Login API登入後，複製回傳的token(不包含雙引號)，貼入右方的Authorize完成授權後，即可操作需授權的API",
                });

                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/NETCoreBase.API.xml");
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/NETCoreBase.Core.xml");
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/NETCoreBase.Common.xml");
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                c.DescribeAllParametersInCamelCase();
                c.CustomSchemaIds(type => type.ToString());
            })
            .AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NET Core Base v1"));
            }

            // Feature 5: Access log middleware (early, skips /health /swagger)
            app.UseMiddleware<AccessLogMiddleware>();

            // Feature 6: Prometheus HTTP metrics collection
            app.UseHttpMetrics();

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var correlationId))
                        diagnosticContext.Set("CorrelationId", correlationId?.ToString());
                };
            });

            app.UseRouting();
            app.UseRateLimiter();
            app.UseCors("BaseCorsPolicy");

            // UseCoreModule = UseAuthentication + TokenBlacklist + UseAuthorization
            app.UseCoreModule();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health").AllowAnonymous();

                // Feature 6: Prometheus scrape endpoint (no auth)
                endpoints.MapMetrics("/prometheus").AllowAnonymous();
            });
        }
    }
}
