using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Model;
using NETCoreBase.Common;
using System.Collections;

namespace NETCoreBase.Common.Formatters
{
    public class ExcelOutputFormatter : OutputFormatter
    {
        private readonly IExcelService _excelService;

        public ExcelOutputFormatter(IExcelService excelService)
        {
            _excelService = excelService;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        protected override bool CanWriteType(Type type)
        {
            return 
                (
                    type.IsGenericType && (
                        type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.GetGenericTypeDefinition() == typeof(PageResult<>)
                    )
                ) ||
                (
                    type.GetInterfaces().Any(x => x.IsGenericType && (
                           x.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||    x.GetGenericTypeDefinition() == typeof(PageResult<>)
                    ))
                ) ||
                type.GetCustomAttribute<ExcelConverterAttribute>() != null;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;
            var excelConverter = context.ObjectType.GetCustomAttribute<ExcelConverterAttribute>();
            if (excelConverter?.Converter != null)
            {
                var converter = Activator.CreateInstance(excelConverter.Converter) as IExcelConverter;
                httpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={WebUtility.UrlEncode(converter.Name)}_{DateTime.Now:yyyyMMddhhmmss}.xlsx");
                var classType = converter.GetType();
                var method = classType.GetMethod("Write");
                method.Invoke(converter, new object[]
                {
                    context.Object,
                    httpContext.Response.BodyWriter.AsStream()
                });
            }
            else
            {
                var genericArgument = context.ObjectType.GetGenericArguments()[0];
                var realGenericArguments = context.Object.GetType().GetGenericArguments()[0];
                var fileName = genericArgument?.Name;
                httpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={WebUtility.UrlEncode(fileName)}_{DateTime.Now:yyyyMMddhhmmss}.xlsx");

                MethodInfo method = null;
                if (context.Object.GetType().Name == typeof(PageResult<>).Name)
                {
                    method = typeof(IExcelService).GetMethod(nameof(_excelService.GetDataListFromPageResult));
                }
                else
                {
                    method = typeof(IExcelService).GetMethod(nameof(_excelService.GetDataList));
                }
                var generic = method.MakeGenericMethod(genericArgument);
                var args = new object[]
                {
                            context.Object,
                            genericArgument.Name,
                            httpContext.Response.BodyWriter.AsStream()
                };
                generic.Invoke(_excelService, args);
            }
            return Task.CompletedTask;
        }
    }
}
