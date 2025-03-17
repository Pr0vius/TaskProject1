
using MediatR;
using ms.auth.domain.Interfaces;
using ms.rabbitmq.Events;
using ms.rabbitmq.Producers;

namespace ms.auth.application.Commands.Handlers
{
    public class CreateAuthAccountCommandHandler : IRequestHandler<CreateAuthAccountCommand, string>
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IProducer _producer;

        public CreateAuthAccountCommandHandler(IAuthRepository authRepository, ITokenService tokenService, IProducer producer)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _producer = producer;
        }

        public async Task<string> Handle(CreateAuthAccountCommand request, CancellationToken cancellationToken)
        {

            var account = await _authRepository.SaveAccount(request.username, Hash(request.password));

            var accountEvent = new AuthAccountCreatedEvent() { Id = account.Id, UserName = account.Username, Email = request.email };
            await _producer.Produce(accountEvent);

            return _tokenService.CreateToken(account.Id, account.Username, account.Role);

        }

        private string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }
    }
}
