using MediatR;
using ms.user.domain.Entities;

namespace ms.user.application.Commands
{
    public record CreateUserCommand(Guid accountId, string username, string email) : IRequest<User>;
}
