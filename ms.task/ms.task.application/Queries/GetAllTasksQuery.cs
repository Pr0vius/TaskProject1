using MediatR;
using ms.task.domain.Entities;

namespace ms.task.application.Queries
{
    public record GetAllTasksQuery(Guid userId) : IRequest<List<UserTask>>;
}
