using ApplicationService.Features.Commands.CommandHandlers.Users;
using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Repositories;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Mocks;

namespace UnitTest.Application.User.Command
{
    public class CreateUserCommandHandlerTest
    {

        private readonly Mock<IUserRepository> _mock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTest()
        {
            _mock = MockUserRepository.GetUserRepository();
            _handler = new CreateUserCommandHandler(_mock.Object);
        }

        [Fact]
        public async Task When_CreateUser_Should_Return_Unit()
        {
            // Arrange
            var user = new CreateUserCommand("example@example.com", "example", "example");

            // Act
            var result = await _handler.Handle(user, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Unit>();
        }
    }
}
