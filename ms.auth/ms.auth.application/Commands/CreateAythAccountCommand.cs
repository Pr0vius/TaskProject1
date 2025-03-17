using MediatR;

namespace ms.auth.application.Commands
{
    public record CreateAuthAccountCommand(string username, string password, string email) : IRequest<string>;
}
