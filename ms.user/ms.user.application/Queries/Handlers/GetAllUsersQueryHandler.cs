using MediatR;
using ms.user.domain.Entities;
using ms.user.domain.Interfaces;

namespace ms.user.application.Queries.Handlers
{
    public class GetAllUsersQueryHandler: IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetAllAsync();
        }
    }
}