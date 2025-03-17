using System.Net;

namespace ms.auth.api.Helpers
{
    public class ExceptionResponse : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public ExceptionResponse(string message = "Unhandled exception", HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }
        public ExceptionResponse(string message = "Unhandled exception") : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
