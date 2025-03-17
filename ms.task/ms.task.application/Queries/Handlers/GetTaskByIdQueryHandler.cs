using MediatR;
using ms.task.domain.Entities;
using ms.task.domain.Interfaces;

namespace ms.task.application.Queries.Handlers
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, UserTask>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTaskByIdQueryHandler(ITaskRepository taskRepository) {
            _taskRepository = taskRepository;
        }

        public async Task<UserTask> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _taskRepository.GetByTaskId(request.UserId, request.TaskId);
        }
    }
}
