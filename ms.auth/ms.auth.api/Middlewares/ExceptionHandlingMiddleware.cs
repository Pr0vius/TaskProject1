using ms.auth.api.Helpers;
using ms.auth.api.Responses;
using System.Net;

namespace ms.auth.api.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.TraceIdentifier;

            context.Response.Headers["X-Trace-ID"] = traceId;
            var request = context.Request;
            var controllerName = context.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            var actionName = context.GetRouteValue("action")?.ToString() ?? "UnknownAction";
            request.EnableBuffering();
            var bodyAsString = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            try
            {
                await _next(context);
            }
            catch (ExceptionResponse ex)
            {
                _logger.LogWarning(
                    "TraceID: {TraceId} | " +
                    "Controller: {Controller} | " +
                    "Action: {Action} | " +
                    "Method: {Method} | " +
                    "URL: {Url} | " +
                    "Query: {Query} | " +
                    "StatusCode: {StatusCode} | " +
                    "Body: {body}",
                    traceId,
                    controllerName,
                    actionName,
                    request.Method,
                    request.Path,
                    request.QueryString,
                    ((int)ex.StatusCode),
                    bodyAsString
                );

                var errorResponse = new ApiResponse<object?>(null, ex.Message, ex.StatusCode);
                context.Response.StatusCode = (int)ex.StatusCode;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "None";
                _logger.LogError(ex,
                    "Unhandled Exception: {Message}. | " +
                    "Path: {Path}. | " +
                    "Method: {Method}. | " +
                    "QueryString: {QueryString}. | " +
                    "Body: {body}. | " +
                    "User: {User}. | " +
                    "ClientIP: {ClientIP}. | " +
                    "TraceID: {TraceID}. | " +
                    "StackTrace: {StackTrace}. | " +
                    "InnerException: {InnerException}.",
                    ex.Message,
                    context.Request.Path,
                    context.Request.Method,
                    context.Request.QueryString,
                    bodyAsString,
                    context.User?.Identity?.Name ?? "Anonymous",
                    context.Connection.RemoteIpAddress?.ToString(),
                    traceId,
                    ex.StackTrace,
                    innerExceptionMessage
                );

                var errorResponse = new ApiResponse<object?>(null, ex.Message, HttpStatusCode.InternalServerError);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
