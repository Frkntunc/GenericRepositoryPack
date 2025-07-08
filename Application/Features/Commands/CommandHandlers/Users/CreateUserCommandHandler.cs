using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using Domain.Events;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandHandlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,Unit>
    {
        private readonly IEventStore _eventStore;
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(IEventStore eventStore, IUserRepository userRepository)
        {
            _eventStore = eventStore;
            this.userRepository = userRepository;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            await userRepository.AddAsync(user);

            // 2. Event oluştur
            var userCreatedEvent = new UserCreatedEvent(user.Id.ToString(), user.Email, $"{user.FirstName} {user.LastName}");

            // 3. Event'i kaydet
            await _eventStore.SaveAsync(userCreatedEvent);

            return Unit.Value;
        }
    }
}
