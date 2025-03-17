using MediatR;
using ms.task.domain.Entities;

namespace ms.task.application.Commands
{
    public record CreateTaskCommand(Guid UserId, string Name, string Description): IRequest<UserTask>;
}
