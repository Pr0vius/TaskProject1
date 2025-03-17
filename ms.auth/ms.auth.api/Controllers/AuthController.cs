using MediatR;
using Microsoft.AspNetCore.Mvc;
using ms.auth.api.Helpers;
using ms.auth.api.Responses;
using ms.auth.application.Commands;
using ms.auth.application.Queries;
using ms.auth.application.Requests;
using System.Net;

namespace ms.auth.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] AccountRequest account)
        {

            try
            {
                var token = await _mediator.Send(new GetAuthenticationTokenQuery(account.UserName, account.Password));
                return ApiResponse<object>.Success(new
                {
                    token
                }, "Logged in succesfully", HttpStatusCode.OK);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is UnauthorizedAccessException)
            {
                throw new ExceptionResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest accountRequest)
        {
            try
            {
                var token = await _mediator.Send(new CreateAuthAccountCommand(accountRequest.UserName, accountRequest.Password, accountRequest.Email));
                return ApiResponse<object>.Success(new { token }, "Registration succesfull", HttpStatusCode.Created);

            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is UnauthorizedAccessException)
            {
                throw new ExceptionResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }
    }
}
