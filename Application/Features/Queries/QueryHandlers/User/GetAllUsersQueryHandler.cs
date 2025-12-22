using ApplicationService.Features.Queries.QueryRequests.User;
using ApplicationService.Repositories;
using Domain.Entities;
using MediatR;
using Shared.Constants;
using Shared.DTOs.Common;
using Shared.DTOs.User;
using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Queries.QueryHandlers.User
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ServiceResponse<List<UserDto>>>
    {
        private readonly IUserRepository userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<ServiceResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await userRepository.GetAllAsync();

            var userList = new List<UserDto>();

            foreach (var item in users)
            {
                userList.Add(new UserDto
                {
                    Id = item.Id,
                    Email = item.Email,
                    Name = item.FirstName + " " + item.LastName
                });
            }

            return ServiceResponse<List<UserDto>>.CreateResponse(userList, ResponseCodes.Success);
        }
    }
}
