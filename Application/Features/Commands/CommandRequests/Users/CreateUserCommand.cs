using ApplicationService.Features.Common;
using ApplicationService.Features.Common.Application.Common.Behaviors;
using MediatR;
using Shared.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandRequests.Users
{
    [EnableRetryAndDlq]
    public class CreateUserCommand : Command, IRequest<ServiceResponse>
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
