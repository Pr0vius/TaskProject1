using MediatR;
using ms.task.domain.Entities;
using ms.task.domain.Interfaces;

namespace ms.task.application.Commands.Handlers
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, UserTask>
    {
        private readonly ITaskRepository _taskRepository;
        public CreateTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<UserTask> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            return await _taskRepository.CreateTask(request.UserId, request.Name, request.Description);
        }
    }
}
