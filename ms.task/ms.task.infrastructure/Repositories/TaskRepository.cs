using Microsoft.EntityFrameworkCore;
using ms.task.domain.Entities;
using ms.task.domain.Interfaces;
using ms.task.infrastructure.Data;

namespace ms.task.infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TasksDBContext _ctx;

        public TaskRepository(TasksDBContext tasksContext)
        {
            _ctx = tasksContext;
        }

        public async Task<List<UserTask>> GetAllByUserId(Guid userId)
        {
            var res = await _ctx.Tasks.Where(t => t.UserId == userId).ToListAsync();
            return res;
        }

        public async Task<UserTask> GetByTaskId(Guid userId, Guid taskId)
        {
            var res = await _ctx.Tasks.FindAsync(taskId);
            if (res == null || res.UserId != userId) throw new KeyNotFoundException("Task not found");
            return res;
        }

        public async Task<UserTask> CreateTask(Guid userId, string name, string description)
        {
            var newTask = new UserTask() { UserId = userId, Name = name, Description = description };

            await _ctx.Tasks.AddAsync(newTask);
            if(_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync();

            return newTask;
        }
        public async Task<UserTask> ChangeTaskState(Guid userId, Guid taskId)
        {
            var task = await _ctx.Tasks.FindAsync(taskId);
            if (task == null || task.UserId != userId) throw new KeyNotFoundException("Task not found");

            task.IsCompleted = !task.IsCompleted;

            if (_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync();
            return task;
        }
        public async Task<bool> DeleteTask(Guid userId, Guid taskId)
        {
            var task = await _ctx.Tasks.FindAsync(taskId);

            if (task == null || task.UserId != userId) throw new KeyNotFoundException("Task not found");

            _ctx.Tasks.Remove(task);

            if(_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
