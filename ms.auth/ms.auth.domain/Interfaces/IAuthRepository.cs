using ms.auth.domain.Entities;

namespace ms.auth.domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthenticationAccount> SaveAccount(string username, string password);

        Task<AuthenticationAccount> GetAccountByUsername(string username);
    }
}
