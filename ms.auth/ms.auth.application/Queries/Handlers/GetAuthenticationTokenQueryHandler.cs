using MediatR;
using ms.auth.domain.Interfaces;


namespace ms.auth.application.Queries.Handlers
{
    public class GetAuthenticationTokenQueryHandler : IRequestHandler<GetAuthenticationTokenQuery, string>
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthRepository _authRepository;
        public GetAuthenticationTokenQueryHandler(ITokenService tokenService, IAuthRepository authRepository)
        {
            _tokenService = tokenService;
            _authRepository = authRepository;
        }

        public async Task<string> Handle(GetAuthenticationTokenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var acc = await _authRepository.GetAccountByUsername(request.Username);

                if (!ComparePasswords(request.Password, acc.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid Credentials");

                return _tokenService.CreateToken(acc.Id, acc.Username, acc.Role);
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
        }

        private bool ComparePasswords(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}
