using ApplicationService.Features.Queries.QueryRequests.User;
using ApplicationService.Repositories;
using MediatR;
using Shared.Constants;
using Shared.DTOs.Common;
using Shared.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Queries.QueryHandlers.User
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<ServiceResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.Id);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FirstName + " " + user.LastName
            };

            return ServiceResponse<List<UserDto>>.CreateResponse(userDto, ResponseCodes.Success);
        }
    }
}
