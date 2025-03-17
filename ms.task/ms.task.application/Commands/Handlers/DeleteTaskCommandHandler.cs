using MediatR;
using ms.task.domain.Interfaces;

namespace ms.task.application.Commands.Handlers
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskRepository _taskRepository;
        public DeleteTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            return await _taskRepository.DeleteTask(request.UserId, request.TaskId);
        }
    }
}
