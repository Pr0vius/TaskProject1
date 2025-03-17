using MediatR;
using ms.user.domain.Entities;
using ms.user.domain.Interfaces;

namespace ms.user.application.Queries.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        IUserRepository _userRepository;
        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var res = await _userRepository.GetByIdAsync(request.id);

            if (res == null) throw new KeyNotFoundException("User not found");

            return res;
        }
    }
}
