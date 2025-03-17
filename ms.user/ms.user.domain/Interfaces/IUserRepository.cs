
using ms.user.domain.Entities;

namespace ms.user.domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task<User> UpdateUser(Guid id, User user);
        Task<User> CreateUser(string username, Guid accountId, string email);
    }
}
