
using Microsoft.EntityFrameworkCore;
using ms.auth.domain.Entities;
using ms.auth.domain.Interfaces;
using ms.auth.infrastructure.Data;

namespace ms.auth.infrastructure.Repositories
{
    public class AuthRepository(AuthAccountsDBContext context) : IAuthRepository
    {
        private readonly AuthAccountsDBContext _ctx = context;

        public async Task<AuthenticationAccount> SaveAccount(string username, string password)
        {
            var existingAccount = await _ctx.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(acc => acc.Username == username.ToLower());

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Username already exist.");
            }

            var account = new AuthenticationAccount(username.ToLower(), password);
            var res = await _ctx.Accounts.AddAsync(account);

            if (_ctx.ChangeTracker.HasChanges())
                await _ctx.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<AuthenticationAccount> GetAccountByUsername(string username)
        {
            var account = await _ctx.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(acc => String.Equals(acc.Username, username.ToLower()));
            if (account == null) throw new KeyNotFoundException("User not found");
            return account;
        }
    }
}