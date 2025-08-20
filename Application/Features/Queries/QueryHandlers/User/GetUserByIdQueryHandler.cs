using ApplicationService.Features.Queries.QueryRequests.User;
using ApplicationService.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTO;

namespace ApplicationService.Features.Queries.QueryHandlers.User
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.Id);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FirstName + " " + user.LastName
            };

            return userDto;
        }
    }
}
