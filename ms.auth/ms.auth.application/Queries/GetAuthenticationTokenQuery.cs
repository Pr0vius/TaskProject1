using MediatR;

namespace ms.auth.application.Queries
{
    public record GetAuthenticationTokenQuery(string Username, string Password) : IRequest<string>;

}
