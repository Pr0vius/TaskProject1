using MediatR;
using ms.task.domain.Entities;

namespace ms.task.application.Queries
{
    public record GetTaskByIdQuery(Guid UserId, Guid TaskId) : IRequest<UserTask>;
}
