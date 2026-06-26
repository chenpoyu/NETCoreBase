using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NETCoreBase.Common.Middleware
{
    /// <summary>
    /// Logs structured access-log entries after each request completes.
    /// Skips /health and /swagger paths.
    /// </summary>
    public class AccessLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessLogMiddleware> _logger;

        public AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip health and swagger paths
            if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            var correlationId = context.Request.Headers["X-Correlation-Id"].ToString()
                                ?? context.TraceIdentifier;
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogInformation(
                "method={Method} path={Path} status={Status} duration={Duration}ms ip={Ip} correlationId={CorrelationId}",
                context.Request.Method,
                path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds,
                ip,
                correlationId);
        }
    }
}
