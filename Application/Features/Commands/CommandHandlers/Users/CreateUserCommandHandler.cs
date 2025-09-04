using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Features.Queries.QueryRequests.User;
using ApplicationService.Repositories;
using Domain.Factories;
using MassTransit;
using MediatR;
using Shared.Enums;
using Shared.Events;
using Shared.Static;

namespace ApplicationService.Features.Commands.CommandHandlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,Unit>
    {
        private readonly IUserRepository userRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateUserCommandHandler(IUserRepository userRepository, IPublishEndpoint publishEndpoint)
        {
            this.userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = UserFactory.Create(
                request.Email,
                request.FirstName,
                request.LastName,
                "hashpassword"
            );

            await userRepository.AddAsync(user);
            await _publishEndpoint.Publish(new CacheInvalidatedEvent(CacheKeys.GetAllUsers), cancellationToken);

            return Unit.Value;
        }
    }
}
