using MediatR;
using ms.task.domain.Entities;

namespace ms.task.application.Commands
{
    public record ChangeTaskStateCommand(Guid UserId, Guid TaskId) : IRequest<UserTask>;
}
