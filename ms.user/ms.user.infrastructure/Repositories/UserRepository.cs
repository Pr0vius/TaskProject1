using Microsoft.EntityFrameworkCore;
using ms.user.domain.Entities;
using ms.user.domain.Interfaces;
using ms.user.infrastructure.Data;

namespace ms.user.infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _ctx;
        public UserRepository(UserDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _ctx.Users.FindAsync(id);
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _ctx.Users.ToListAsync();
        }
        public async Task<User> UpdateUser(Guid id, User newUser)
        {
            var user = await _ctx.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            
            user.UpdateEmail(newUser.Email);

            await _ctx.SaveChangesAsync();

            return user;
        }
        public async Task<User> CreateUser(string username, Guid accountId, string email)
        {
            var res = await _ctx.Users.AddAsync(new User(username, accountId, email));

            if (_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync();
            return res.Entity;
        }
    }
}
