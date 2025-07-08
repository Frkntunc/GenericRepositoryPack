using ApplicationService.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandRequests.Users
{
    public class CreateUserCommand : Command, IRequest<Unit>
    {
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public CreateUserCommand(string email, string firstName, string lastName)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }

}
