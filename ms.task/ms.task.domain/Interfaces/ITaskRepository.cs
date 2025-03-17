using ms.task.domain.Entities;
namespace ms.task.domain.Interfaces
{
    public interface ITaskRepository
    {

        Task<List<UserTask>> GetAllByUserId(Guid UserId);
        Task<UserTask> GetByTaskId(Guid userId ,Guid TaskId);
        Task<UserTask> CreateTask(Guid userId, string name, string description);
        Task<UserTask> ChangeTaskState(Guid userId, Guid taskId);
        Task<bool> DeleteTask(Guid userId, Guid taskId);
    }
}
