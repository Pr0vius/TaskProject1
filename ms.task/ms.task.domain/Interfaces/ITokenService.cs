using System.Security.Claims;

namespace ms.task.domain.Interfaces
{
    public interface ITokenService
    {
        ClaimsPrincipal DecodeToken(string token);
    }
}
