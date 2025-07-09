using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using Domain.Entities;
using MediatR;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandHandlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,Unit>
    {
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Status = StatusType.Available,
                PasswordHash = "sadfghj",
                LoginTryCount = 0,
                IsBlocked = false
            };

            await userRepository.AddAsync(user);

            return Unit.Value;
        }
    }
}
