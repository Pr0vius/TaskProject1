
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ms.user.api.Responses
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(T data, string message, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static IActionResult Success(T data, string message, HttpStatusCode statusCode)
        {
            return new ObjectResult(new ApiResponse<T>(data, message, statusCode)) { StatusCode = ((int)statusCode) };
        }

        public static IActionResult Error(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var response = new ApiResponse<object?>(null, message, statusCode);
            return new ObjectResult(response) { StatusCode = (int)statusCode };
        }
    }
}
