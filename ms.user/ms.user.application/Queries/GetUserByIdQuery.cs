using MediatR;
using ms.user.domain.Entities;

namespace ms.user.application.Queries
{
    public record GetUserByIdQuery(Guid id): IRequest<User>;
}
