
using MediatR;
using ms.task.domain.Entities;
using ms.task.domain.Interfaces;

namespace ms.task.application.Commands.Handlers
{
    public class ChangeTaskStateCommandHandler : IRequestHandler<ChangeTaskStateCommand, UserTask>
    {
        private readonly ITaskRepository _taskRepository;

        public ChangeTaskStateCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<UserTask> Handle(ChangeTaskStateCommand request, CancellationToken cancellationToken)
        {
            return await _taskRepository.ChangeTaskState(request.UserId, request.TaskId);
        }
    }
}
