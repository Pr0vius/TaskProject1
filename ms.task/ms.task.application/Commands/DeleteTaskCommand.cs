using MediatR;

namespace ms.task.application.Commands
{
    public record DeleteTaskCommand(Guid UserId, Guid TaskId): IRequest<bool>;
}
