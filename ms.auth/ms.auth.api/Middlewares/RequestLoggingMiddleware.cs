using System.Diagnostics;

namespace ms.auth.api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var traceId = context.TraceIdentifier;
            context.Response.Headers["X-Trace-ID"] = traceId;

            var request = context.Request;
            request.EnableBuffering();
            var bodyAsText = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            await _next(context);
            stopwatch.Stop();

            var controllerName = context.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            var actionName = context.GetRouteValue("action")?.ToString() ?? "UnknownAction";
            if (context.Response.StatusCode < 400)
                _logger.LogInformation(
                    "TraceID: {TraceId} | " +
                    "Controller: {Controller} | " +
                    "Action: {Action} | " +
                    "Method: {Method} | " +
                    "URL: {Url} | " +
                    "Query: {Query} | " +
                    "StatusCode: {StatusCode} | " +
                    "ProcessingTime: {ProcessingTime} ms | " +
                    "Body: {body}",
                    traceId,
                    controllerName,
                    actionName,
                    request.Method,
                    request.Path,
                    request.QueryString,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    bodyAsText
                );
        }
    }
}
