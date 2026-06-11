using System;
using System.IO;
using System.Reflection;
using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NETCoreBase.Common;
using NETCoreBase.Common.Filter.Swagger;
using NETCoreBase.Common.Model;
using NETCoreBase.Common.Policies;
using NETCoreBase.Core;
using Microsoft.EntityFrameworkCore;
using NETCoreBase.Common.Filter;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using NETCoreBase.Common.Formatters;
using Newtonsoft.Json.Serialization;
using NETCoreBase.Common.Services;
using NETCoreBase.Common.Interfaces;
using Serilog;
using MediatR;
using NETCoreBase.Core.Behaviors;

namespace NETCoreBase.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new EFModule(Configuration.GetConnectionString("DefaultConnection")));
            builder.RegisterModule(new AutoMapperModule());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("BaseCorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(orign => true)
                    .AllowCredentials()
                );
            });

            var coreModuleOptions = (JwtTokenConfig) Configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfig>();

            services.AddCoreModule(coreModuleOptions);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<MediatorModule>();
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            });

            services.AddControllers(c =>
            {
                c.RespectBrowserAcceptHeader = true;
                c.Filters.Add(new AuthorizeFilter(JwtAuthPolicy.PolicyName));
                c.Filters.Add(new AuthorizeFilter(PermissionPolicy.PolicyName));
                // c.Filters.Add(typeof(ResultMiddleware));
                c.Filters.Add(new HttpResponseExceptionFilter());

            }).AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<MediatorModule>();
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            })
            // .AddNewtonsoftJson(o => 
            // { 
            //     o.SerializerSettings.ContractResolver = new DefaultContractResolver
            //     {
            //         NamingStrategy = new SnakeCaseNamingStrategy()
            //     };
            // })
            .AddExcelOutputFormatter()
            //.AddDataAnnotationsLocalization(options => {
            //    options.DataAnnotationLocalizerProvider = (type, factory) =>
            //        factory.Create(typeof(DataAnnotationResource));
            //})
            ;

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
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });
                // c.OperationFilter<ResponseHeadersFilter>();
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                c.DescribeAllParametersInCamelCase();
                c.CustomSchemaIds(type => type.ToString());
            })
            .AddSwaggerGenNewtonsoftSupport();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NET Core Base v1"));
            }


            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors("BaseCorsPolicy");
            app.UseCoreModule();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                //endpoints.MapControllers();
            });
        }
    }
}
