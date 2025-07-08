using Domain.Events;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandHandlers.Users
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly IMongoCollection<UserReadModel> _users;

        public UserCreatedEventHandler(MongoDbContext mongoDbContext)
        {
            _users = mongoDbContext.Users;
        }

        public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
        {
            var readModel = new UserReadModel
            {
                Id = @event.UserId,
                Email = @event.Email,
                FullName = @event.FullName
            };

            await _users.InsertOneAsync(readModel, cancellationToken: cancellationToken);
        }
    }

}
