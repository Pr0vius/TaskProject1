using MediatR;
using ms.user.domain.Entities;

namespace ms.user.application.Queries
{
    public record GetAllUsersQuery() : IRequest<List<User>>;
}
